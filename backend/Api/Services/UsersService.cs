using System.IdentityModel.Tokens.Jwt;
using System.Transactions;
using BCrypt.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class UserService
{
  private readonly AppDbContext _context;
  private readonly TokenService _tokenService;

  public UserService(AppDbContext context, TokenService tokenService)
  {
    _context = context;
    _tokenService = tokenService;
  }
  public async Task<List<UserResponseDto>> GetAllUsersAsync()
  {
    var users = await _context.Users
      .Select(u => new
      {
        u.Id,
        u.Email,
        u.SpecializationType,
        u.CreatedAt,
        u.IsActive,
      })
      .ToListAsync();


    var userData = await _context.UserData
      .Select(ud => new 
      { 
        ud.UserId, 
        ud.FirstName, 
        ud.LastName,
        ud.Skills
      })
      .ToListAsync();

    var userDataDict = userData.ToDictionary(x => x.UserId);


    var result = users.Select(u =>
    {
      userDataDict.TryGetValue(u.Id, out var ud);
      return new UserResponseDto
      {
        Id = u.Id,
        Email = u.Email,
        Specialization = u.SpecializationType,
        Skills = ud.Skills,
        FirstName = ud?.FirstName,
        LastName = ud?.LastName,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt
      };
    }).ToList();

    return result;
  }
  public async Task<UserResponseDto> GetCurrentUserAsync(Guid userId)
  {

    var user = await _context.Users
      .Where(u => u.Id == userId)
      .Select(u => new UserResponseDto
      {
        Id = u.Id,
        Email = u.Email,
        Specialization = u.SpecializationType,
        Skills = u.UserData.Skills,
        FirstName = u.UserData.FirstName,
        LastName = u.UserData.LastName,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt
      })
      .FirstOrDefaultAsync()  
      ?? throw new KeyNotFoundException($"Not found user with ID {userId}");

    return user;
  }
  public async Task<bool> UpdateCurrentUserAsync(Guid userId, UpdateCurrentUserDto request)
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

    userData.FirstName = string.IsNullOrWhiteSpace(request.FirstName) ? userData.FirstName : request.FirstName;
    userData.LastName = string.IsNullOrWhiteSpace(request.LastName) ? userData.LastName : request.LastName;
    userData.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? userData.PhoneNumber : request.PhoneNumber;
    userData.Skills = string.IsNullOrWhiteSpace(request.Skills) ? userData.Skills : request.Skills;
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
  public async Task<bool> DeleteCurrentUserAsync(Guid userId)
  {
    if (userId == Guid.Empty)
      throw new ArgumentException("UserId cannot be null or empty");

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
  public async Task<bool> LogoutUserAsync(Guid userId, string deviceIp)
  {
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId)
      ?? throw new UnauthorizedAccessException($"User with ID {userId} not found.");

    var isSessionsRevoked = await _tokenService.RevokeSessionAsync(userId, deviceIp);
    if (!isSessionsRevoked)
      throw new KeyNotFoundException($"No active sessions found for user with ID {userId} from IP {deviceIp}.");

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
      SpecializationType = request.SpecializationType ?? new List<string>()
    };

    var userData = new UserDataModel
    {
      FirstName = request.FirstName,
      LastName = request.LastName,
      PhoneNumber = request.PhoneNumber,
      Skills = request.Skills,
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
