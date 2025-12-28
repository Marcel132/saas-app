using Microsoft.EntityFrameworkCore;

public class AuthService
{

  private readonly AppDbContext _context;
  private readonly TokenService _tokenService;
  public AuthService(
    AppDbContext context,
    TokenService tokenService
  )
  {
    _context = context;
    _tokenService = tokenService;
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
        Guid.Empty, 
        HttpStatusCodes.AuthCodes.InvalidCredentials
      );
    }

    if(!user.IsActive)
      return new AuthLoginResult(
        false,
        Guid.Empty, 
        HttpStatusCodes.AuthCodes.AccountBlocked
      );
    // ResetFailedLoginAttempts(user);

    return new AuthLoginResult(
      true, 
      user.Id,
      HttpStatusCodes.AuthCodes.Success
    );
  }

  public async Task<RegisterResponseDto> RegisterUserAsync(RegisterRequestDto request)
  {
    var existingUser = await _context.Users
      .FirstOrDefaultAsync(u => u.Email == request.Email);
      
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

  public async Task<bool> LogoutUserAsync(Guid userId, string? deviceIp)
  {
    var isSessionsRevoked = await _tokenService.RevokeSessionAsync(userId, deviceIp);
    if(!isSessionsRevoked)
      throw new KeyNotFoundException("No active sessions");

    return true;
  }
}