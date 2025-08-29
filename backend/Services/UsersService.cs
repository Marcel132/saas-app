using System.Transactions;
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
  public async Task<List<UsersModel>> GetAllUsersAsync()
  {
    var users = await _context.users
    .Include(u => u.Sessions)
    .Include(u => u.UserData)
    .Include(u => u.Opinions)
    .Include(u => u.ApiLogs)
    .ToListAsync();

    return users;
  }

  public async Task<UsersModel> GetUserByIdAsync(int userId)
  {
    var user = await _context.users
      .Include(u => u.Sessions)
      .Include(u => u.UserData)
      .Include(u => u.Opinions)
      .Include(u => u.ApiLogs)
      .FirstOrDefaultAsync(u => u.Id == userId);


    if (user == null)
    {
      throw new KeyNotFoundException($"User with ID {userId} not found.");
    }
    else
    {
      return user;
    }
  }

  public async Task<bool> UpdateUserAsync(int userId, UpdateUserModel userModel)
  {
    if (userId <= 0 || userModel == null)
    {
      throw new ArgumentException("Invalid user ID or update data.");
    }
    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
      var user = await _context.users.FindAsync(userId) ?? throw new KeyNotFoundException($"User with ID {userId} not found.");
      var userData = await _context.user_data.FirstOrDefaultAsync(ud => ud.UserId == userId) ?? throw new KeyNotFoundException($"User data for user with ID {userId} not found.");

      user.PasswordHash = string.IsNullOrWhiteSpace(userModel.PasswordHash) ? user.PasswordHash : userModel.PasswordHash;
      if (Enum.IsDefined(typeof(RoleType), userModel.Role))
        user.Role = userModel.Role;
      user.IsActive = userModel.IsActive;

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
    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {

      var user = await _context.users.FindAsync(userId);
      if (user == null)
      {
        throw new KeyNotFoundException($"User with ID {userId} not found.");
      }

      _context.users.Remove(user);

      var userSessions = await _context.sessions.Where(s => s.UserId == userId).ToListAsync();
      _context.sessions.RemoveRange(userSessions);

      var userData = await _context.user_data.FirstOrDefaultAsync(ud => ud.UserId == userId);
      if (userData != null)
      {
        _context.user_data.Remove(userData);
      }

      var userOpinions = await _context.opinions.Where(o => o.TargetId == userId).ToListAsync();
      _context.opinions.RemoveRange(userOpinions);

      var userApiLogs = await _context.api_logs.Where(al => al.UserId == userId).ToListAsync();
      _context.api_logs.RemoveRange(userApiLogs);

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

  public async Task<UsersModel> AuthenticateUserAsync(string email, string password)
  {
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
      throw new ArgumentException("Email and password must be provided.");
    }

    var user = await _context.users
      .Include(u => u.UserData)
      .FirstOrDefaultAsync(u => u.Email == email);

    if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
    {
      throw new UnauthorizedAccessException("Invalid email or password.");
    }

    return user;
  }

  public async Task<bool> LogoutUserAsync(string email, string deviceIp)
  {
    ArgumentNullException.ThrowIfNull(email);

    var user = await _context.users.FirstOrDefaultAsync(u => u.Email == email)
      ?? throw new KeyNotFoundException($"User with email {email} not found.");

    var sessions = await _context.sessions
      .Where(s => s.UserId == user.Id && !s.Revoked && s.Ip == deviceIp)
      .ToListAsync();

    if (sessions == null || sessions.Count == 0)
    {
      throw new KeyNotFoundException("No active sessions found for this user.");
    }

    foreach (var sess in sessions)
    {
      sess.Revoked = true;
    }

    await _context.SaveChangesAsync();
    return true;
  }
  public async Task<UsersModel> RegisterModelAsync(RegisterRequestModel model)
  {

    ArgumentNullException.ThrowIfNull(model);

    var user = model.User;
    ArgumentNullException.ThrowIfNull(user);

    var userData = model.UserData;
    ArgumentNullException.ThrowIfNull(userData);

    if (string.IsNullOrWhiteSpace(user.Email)
    || string.IsNullOrWhiteSpace(user.PasswordHash)
    || string.IsNullOrWhiteSpace(userData.FirstName)
    || string.IsNullOrWhiteSpace(userData.LastName)
    || string.IsNullOrWhiteSpace(userData.PhoneNumber)
    || string.IsNullOrWhiteSpace(userData.Country)
    || string.IsNullOrWhiteSpace(userData.PostalCode)
    || string.IsNullOrWhiteSpace(userData.Street)
    )
    {
      throw new ArgumentNullException("Requested data is null or required fields are missing.");
    }

    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
      var existingUser = await _context.users.FirstOrDefaultAsync(u => u.Email == user.Email);
      if (existingUser != null)
      {
        throw new ArgumentException("User with this email already exists.");
      }

      var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
      user.PasswordHash = hashedPassword;
      _context.users.Add(user);
      await _context.SaveChangesAsync();

      userData.UserId = user.Id; // Ensure UserId is set correctly
      var existingUserData = await _context.user_data.FirstOrDefaultAsync(ud => ud.UserId == userData.UserId);
      if (existingUserData != null)
      {
        throw new ArgumentException("User data for this user already exists.");
      }
      _context.user_data.Add(userData);
      await _context.SaveChangesAsync();
      await transaction.CommitAsync();
      return user;
    }
    catch (System.Exception)
    {
      await transaction.RollbackAsync();
      throw;
    }

  }

}
