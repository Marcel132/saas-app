using System.IdentityModel.Tokens.Jwt;
using System.Transactions;
using BCrypt.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class UserService
{
  private readonly AppDbContext _context;
  public UserService(AppDbContext context)
  {
    _context = context;
  }
  public async Task<List<UserDto>> GetAllUsersAsync()
  {
    var usersData = await _context.Users
      .Include(u => u.UserData)
      .Select(u => new UserDto
      {
      Id = u.Id,
      Email = u.Email,
      Specialization = u.SpecializationType.ToString(),
      Skills = u.SpecializationType == SpecializationEnum.Pentester ? u.Skills : "",
      FirstName = u.UserData.FirstName,
      LastName = u.UserData.LastName
      }).ToListAsync();

    return usersData;
  }
  public async Task<UserDto> GetCurrentUserAsync(int userId)
  {
    if (userId <= 0)
      throw new ArgumentException("User ID must be a positive integer.", nameof(userId));

    var user = await _context.Users
      .Select(u => new UserDto
      {
        Id = u.Id,
        Email = u.Email,
        Specialization = u.SpecializationType.ToString(),
        Skills = u.SpecializationType == SpecializationEnum.Pentester ? u.Skills : "",
        FirstName = u.UserData.FirstName,
        LastName = u.UserData.LastName
      })
      .FirstOrDefaultAsync(u => u.Id == userId)
      ?? throw new KeyNotFoundException($"User with ID {userId} not found.");

    return user;
  }
  public async Task<bool> UpdateUserAsync(int userId, UpdateUserModel userModel)
  {
    if (userId <= 0 || userModel == null)
      throw new ArgumentException("Invalid user ID or update data.");

    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
      var user = await _context.Users.FindAsync(userId) ?? throw new KeyNotFoundException($"User with ID {userId} not found."); 
      var userData = await _context.UserData.FirstOrDefaultAsync(ud => ud.UserId == userId)
        ?? throw new KeyNotFoundException($"User data for user with ID {userId} not found.");

      // user.PasswordHash = string.IsNullOrWhiteSpace(userModel.PasswordHash) ? user.PasswordHash : userModel.PasswordHash;
      // if (Enum.IsDefined(typeof(RoleType), userModel.Role))
      //   user.Role = userModel.Role;
      if (Enum.IsDefined<SpecializationEnum>(userModel.SpecializationType))
        user.SpecializationType = userModel.SpecializationType;
      user.Skills = string.IsNullOrWhiteSpace(userModel.Skills) ? user.Skills : userModel.Skills;
        
      userData.FirstName = string.IsNullOrWhiteSpace(userModel.FirstName) ? userData.FirstName : userModel.FirstName;
      userData.LastName = string.IsNullOrWhiteSpace(userModel.LastName) ? userData.LastName : userModel.LastName;
      userData.PhoneNumber = string.IsNullOrWhiteSpace(userModel.PhoneNumber) ? userData.PhoneNumber : userModel.PhoneNumber;
      userData.City = string.IsNullOrWhiteSpace(userModel.City) ? userData.City : userModel.City;
      userData.Country = string.IsNullOrWhiteSpace(userModel.Country) ? userData.Country : userModel.Country;
      userData.PostalCode = string.IsNullOrWhiteSpace(userModel.PostalCode) ? userData.PostalCode : userModel.PostalCode;
      userData.Street = string.IsNullOrWhiteSpace(userModel.Street) ? userData.Street : userModel.Street;
      userData.CompanyName = string.IsNullOrWhiteSpace(userModel.CompanyName) ? userData.CompanyName : userModel.CompanyName;
      userData.CompanyNip = string.IsNullOrWhiteSpace(userModel.CompanyNip) ? userData.CompanyNip : userModel.CompanyNip;

      userData.IsEmailVerified = userModel.IsEmailVerified;
      userData.IsTwoFactorEnabled = userModel.IsTwoFactorEnabled;
      userData.IsProfileCompleted = userModel.IsProfileCompleted;

      await _context.SaveChangesAsync();
      await transaction.CommitAsync();

      return true;
    }
    catch (System.Exception)
    {
      await transaction.RollbackAsync();
      throw;
    }
  }
  public async Task<bool> DeleteUserAsync(int userId)
  {
    if (userId <= 0)
      throw new ArgumentException("UserId cannot be null or lower than 0");

    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
      var userSessions = await _context.Sessions.Where(s => s.UserId == userId).ToListAsync();
      _context.Sessions.RemoveRange(userSessions);

      var userData = await _context.UserData.FirstOrDefaultAsync(ud => ud.UserId == userId);
      if (userData != null)
      {
        _context.UserData.Remove(userData);
      }

      var userOpinions = await _context.Opinions.Where(o => o.TargetId == userId).ToListAsync();
      _context.Opinions.RemoveRange(userOpinions);

      var userApiLogs = await _context.ApiLogs.Where(al => al.UserId == userId).ToListAsync();
      _context.ApiLogs.RemoveRange(userApiLogs);

      var user = await _context.Users.FindAsync(userId)
      ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
      _context.Users.Remove(user);

      await _context.SaveChangesAsync();
      await transaction.CommitAsync();

      return true;
    }
    catch (System.Exception)
    {
      await transaction.RollbackAsync();
      throw;
    }
  }
  public async Task<UsersModel> AuthenticateUserAsync(LoginRequestModel request)
  {
    if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
      throw new ArgumentException("Email and password must be provided.");
    

    var user = await _context.Users
      .Include(u => u.UserData)
      .FirstOrDefaultAsync(u => u.Email == request.Email)
      ?? throw new UnauthorizedAccessException("Invalid email");

    if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
      throw new UnauthorizedAccessException("Invalid password.");
    
    return user;
  }
  public async Task<bool> LogoutUserAsync(int userId, string deviceIp)
  {
    if (userId <= 0)
      throw new ArgumentException("UserId must be greater than 0");

    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId)
      ?? throw new KeyNotFoundException($"User with ID {userId} not found.");

    var sessions = await _context.Sessions
      .Where(s => s.UserId == user.Id && !s.Revoked && s.Ip == deviceIp)
      .ToListAsync();

    if (sessions.Count == 0)
      throw new KeyNotFoundException("No active sessions found for this user");

    foreach (var sess in sessions)
    {
      sess.Revoked = true;
    }

    await _context.SaveChangesAsync();
    return true;
  }
  public async Task<UsersModel> RegisterUserAsync(UsersModel request)
  {
    ArgumentNullException.ThrowIfNull(request, "Request cannot be null");

    ArgumentNullException.ThrowIfNullOrWhiteSpace(request.Email, "Email must have a value");
    ArgumentNullException.ThrowIfNullOrWhiteSpace(request.Password, "Password must have a value");
    ArgumentNullException.ThrowIfNullOrWhiteSpace(request.SpecializationType.ToString(), "Specialization type must have a value");
    ArgumentNullException.ThrowIfNullOrWhiteSpace(request.UserData.FirstName, "First Name must have a value");
    ArgumentNullException.ThrowIfNullOrWhiteSpace(request.UserData.LastName, "Last Name must have a value");
    ArgumentNullException.ThrowIfNullOrWhiteSpace(request.UserData.PhoneNumber, "Phone Number must have a value");
    ArgumentNullException.ThrowIfNullOrWhiteSpace(request.UserData.Country, "Country must have a value");
    ArgumentNullException.ThrowIfNullOrWhiteSpace(request.UserData.PostalCode, "Postal Code must have a value");
    ArgumentNullException.ThrowIfNullOrWhiteSpace(request.UserData.Street, "Street must have a value");

    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
      var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
      if (existingUser != null)
      {
        throw new ArgumentException("User with this email already exists.");
      }

      var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

      request.Password = hashedPassword;

      if (request.UserData == null)
        throw new ArgumentNullException("User cannot be null");
      

      _context.Users.Add(request);
      await _context.SaveChangesAsync();
      await transaction.CommitAsync();

      return request;
    }
    catch (System.Exception)
    {
      await transaction.RollbackAsync();
      throw;
    }

  }

}
