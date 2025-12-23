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
  var userData = await _context.UserData
    .Select(ud => new { ud.UserId, ud.FirstName, ud.LastName })
    .ToListAsync();

  var userDataDict = userData.ToDictionary(x => x.UserId);

  var users = await _context.Users
    .Select(u => new
    {
      u.Id,
      u.Email,
      u.SpecializationType,
      u.Skills
    })
    .ToListAsync();

  var result = users.Select(u =>
  {
    userDataDict.TryGetValue(u.Id, out var ud);

    return new UserDto
    {
      Id = u.Id,
      Email = u.Email,
      Specialization = u.SpecializationType,
      Skills = u.Skills,
      FirstName = ud?.FirstName ?? string.Empty,
      LastName  = ud?.LastName  ?? string.Empty
    };
  }).ToList();

  return result;
}
  public async Task<UserDto> GetCurrentUserAsync(int userId)
  {
    if (userId <= 0 )
      throw new ArgumentException("User ID must be a positive integer.", nameof(userId));

    var userData = await _context.UserData
      .Select(ud => new { ud.UserId, ud.FirstName, ud.LastName})
      .FirstOrDefaultAsync(ud => ud.UserId == userId)
      ?? throw new KeyNotFoundException($"User data for user with ID {userId} not found."); //Thats never gonna happen

    var user = await _context.Users
      .Select(u => new UserDto
      {
        Id = u.Id,
        Email = u.Email,
        Specialization = u.SpecializationType,
        Skills = u.Skills,
        FirstName = userData.FirstName,
        LastName = userData.LastName
      })
      .FirstOrDefaultAsync(u => u.Id == userId)
      ?? throw new KeyNotFoundException($"User with ID {userId} not found.");

    return user;
  }
  public async Task<bool> UpdateUserAsync(int userId, UpdateUserModel request)
  {
    using var transaction = await _context.Database.BeginTransactionAsync();
    var user = await _context.Users.FindAsync(userId) ?? throw new KeyNotFoundException($"User with ID {userId} not found."); 
    var userData = await _context.UserData.FirstOrDefaultAsync(ud => ud.UserId == userId)
      ?? throw new KeyNotFoundException($"User data for user with ID {userId} not found.");

    // user.PasswordHash = string.IsNullOrWhiteSpace(userModel.PasswordHash) ? user.PasswordHash : userModel.PasswordHash;
    // if (Enum.IsDefined(typeof(RoleType), userModel.Role))
    //   user.Role = userModel.Role;
    user.SpecializationType = request.SpecializationType;
    user.Skills = string.IsNullOrWhiteSpace(request.Skills) ? user.Skills : request.Skills;

    userData.FirstName = string.IsNullOrWhiteSpace(request.FirstName) ? userData.FirstName : request.FirstName;
    userData.LastName = string.IsNullOrWhiteSpace(request.LastName) ? userData.LastName : request.LastName;
    userData.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? userData.PhoneNumber : request.PhoneNumber;
    userData.City = string.IsNullOrWhiteSpace(request.City) ? userData.City : request.City;
    userData.Country = string.IsNullOrWhiteSpace(request.Country) ? userData.Country : request.Country;
    userData.PostalCode = string.IsNullOrWhiteSpace(request.PostalCode) ? userData.PostalCode : request.PostalCode;
    userData.Street = string.IsNullOrWhiteSpace(request.Street) ? userData.Street : request.Street;
    userData.CompanyName = string.IsNullOrWhiteSpace(request.CompanyName) ? userData.CompanyName : request.CompanyName;
    userData.CompanyNip = string.IsNullOrWhiteSpace(request.CompanyNip) ? userData.CompanyNip : request.CompanyNip;

    await _context.SaveChangesAsync();
    await transaction.CommitAsync();

    return true;
  }
  public async Task<bool> DeleteUserAsync(int userId)
  {
    if (userId <= 0)
      throw new ArgumentException("UserId cannot be null or lower than 0");

    using var transaction = await _context.Database.BeginTransactionAsync();

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
  public async Task<AuthLoginResult> AuthenticateUserAsync(LoginRequestModel request)
  {
    if(request == null)
    {
      throw new ArgumentNullException("Request model is not valid");
    }
    if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
      return new AuthLoginResult(
        false,
        null, 
        HttpStatusCodes.AuthCodes.InvalidCredentials
      );
    

    var user = await _context.Users
      .FirstOrDefaultAsync(u => u.Email == request.Email);

    
    if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
    {
      // await RegisterFailedLoginAttempt(request.Email);

      return new AuthLoginResult(
        false,
        null, 
        HttpStatusCodes.AuthCodes.InvalidCredentials
      );
    }

    if(!user.IsActive)
    {
      return new AuthLoginResult(
        false,
        null, 
        HttpStatusCodes.AuthCodes.AccountBlocked
      );
    }
    // ResetFailedLoginAttempts(user);

    return new AuthLoginResult(
      true, 
      user,
      HttpStatusCodes.AuthCodes.Success
    );
  }
  public async Task<bool> LogoutUserAsync(int userId, string deviceIp)
  {
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId)
      ?? throw new UnauthorizedAccessException($"User with ID {userId} not found.");

    var sessions = await _context.Sessions
      .Where(s => s.UserId == user.Id && !s.Revoked && s.Ip == deviceIp)
      .ToListAsync();

    if (sessions.Count == 0)
      return false;

    foreach (var sess in sessions)
    {
      sess.Revoked = true;
    }

    await _context.SaveChangesAsync();
    return true;
  }
  public async Task<RegisterRequestModel> RegisterUserAsync(RegisterRequestModel request)
  {
    if(request == null)
      throw new ArgumentNullException("Request model is not valid");

    if(request.User == null || request.UserData == null)
      throw new ArgumentNullException("User cannot be null");

    if(string.IsNullOrWhiteSpace(request.User.Email) || string.IsNullOrWhiteSpace(request.User.Password))
      throw new ArgumentException("Email and Password must have a value");

    // ArgumentNullException.ThrowIfNullOrWhiteSpace(request.User.Email, "Email must have a value");
    // ArgumentNullException.ThrowIfNullOrWhiteSpace(request.User.Password, "Password must have a value");
    // ArgumentNullException.ThrowIfNullOrWhiteSpace(request.User.SpecializationType.ToString(), "Specialization type must have a value");
    // ArgumentNullException.ThrowIfNullOrWhiteSpace(request.UserData.FirstName, "First Name must have a value");
    // ArgumentNullException.ThrowIfNullOrWhiteSpace(request.UserData.LastName, "Last Name must have a value");
    // ArgumentNullException.ThrowIfNullOrWhiteSpace(request.UserData.PhoneNumber, "Phone Number must have a value");
    // ArgumentNullException.ThrowIfNullOrWhiteSpace(request.UserData.Country, "Country must have a value");
    // ArgumentNullException.ThrowIfNullOrWhiteSpace(request.UserData.PostalCode, "Postal Code must have a value");
    // ArgumentNullException.ThrowIfNullOrWhiteSpace(request.UserData.Street, "Street must have a value");

    using var transaction = await _context.Database.BeginTransactionAsync();

    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.User.Email);
    if (existingUser != null)
    {
      throw new ArgumentException("User with this email already exists.");
    }


    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.User.Password);

    request.User.Password = hashedPassword;
    

    if (request.UserData == null)
      throw new ArgumentNullException("User cannot be null");
    
    _context.Users.Add(request.User);
    await _context.SaveChangesAsync();

    request.UserData.UserId = request.User.Id;

    _context.UserData.Add(request.UserData);
    await transaction.CommitAsync();

    return request;
  }

}
