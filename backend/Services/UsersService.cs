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
  public async Task<bool> UpdateCurrentUserAsync(int userId, UpdateCurrentUserDto request)
  {
    using var transaction = await _context.Database.BeginTransactionAsync();

    var user = await _context.Users.FindAsync(userId) 
      ?? throw new KeyNotFoundException($"User with ID {userId} not found."); 
    
    var userData = await _context.UserData.FirstOrDefaultAsync(ud => ud.UserId == userId)
      ?? throw new KeyNotFoundException($"User data for user with ID {userId} not found.");

    if (!string.IsNullOrWhiteSpace(request.Password))
    {
      var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
      user.Password = hashedPassword;
    }
    user.SpecializationType = request.SpecializationType ?? user.SpecializationType;
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
  public async Task<AuthLoginResult> AuthenticateUserAsync(LoginRequestDto request)
  {
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
      return new AuthLoginResult(
        false,
        null, 
        HttpStatusCodes.AuthCodes.AccountBlocked
      );
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
  public async Task<RegisterResponseDto> RegisterUserAsync(RegisterRequestDto request)
  {
    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
    if (existingUser != null)
      throw new ConflictAppException("A user with the provided email already exists.");

    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);
    
    var user = new UsersModel
    {
      Email = request.Email,
      Password = hashedPassword,
      IsActive = true,
      CreatedAt = DateTime.UtcNow,
      SpecializationType = request.SpecializationType ?? new List<string>(),
      Skills = request.Skills ?? string.Empty

    };

    var userData = new UserDataModel
    {
      FirstName = request.FirstName,
      LastName = request.LastName,
      PhoneNumber = request.PhoneNumber,
      City = request.City,
      Country = request.Country,
      PostalCode = request.PostalCode,
      Street = request.Street,
      CompanyName = request.CompanyName ?? string.Empty,
      CompanyNip = request.CompanyNip ?? string.Empty
    };

    user.UserData = userData; 

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return new RegisterResponseDto
    {
      Id = user.Id
    };
  }

}
