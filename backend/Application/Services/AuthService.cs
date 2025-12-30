using Microsoft.EntityFrameworkCore;

public class AuthService
{

  private readonly AppDbContext _context;
  private readonly TokenService _tokenService;
  private readonly RoleService _roleService;

  // Policy
  private readonly LoginPolicy _loginPolicy;
  public AuthService(
    AppDbContext context,
    TokenService tokenService,
    RoleService roleService,

    LoginPolicy loginPolicy
  )
  {
    _context = context;
    _tokenService = tokenService;
    _roleService = roleService;

    _loginPolicy = loginPolicy;
  }

  public async Task<AuthLoginResult> AuthenticateUserAsync(LoginRequestDto request)
  {
    request.Email = request.Email.Trim().ToLowerInvariant();
    var user = await _context.Users
      .Select(u => new UserLoginDataDto
      {
        Id = u.Id,
        Email = u.Email,
        HashedPassword = u.Password,
        IsActive = u.IsActive
      })
      .FirstOrDefaultAsync(u => u.Email == request.Email);

    _loginPolicy.Validate(user, request.Password);

    var userId = user!.Id;
    
    var userPermissions = await _roleService.GetEffectivePermissions(userId);
    
    return new AuthLoginResult(
      true,
      user.Id,
      userPermissions,
      HttpStatusCodes.AuthCodes.Authorized
    );
  }

  public async Task<RegisterResponseDto> RegisterUserAsync(RegisterRequestDto request)
  {

    request.Email = request.Email.Trim().ToLowerInvariant();
    var existingUser = await _context.Users
      .FirstOrDefaultAsync(u => u.Email == request.Email);
      
    if (existingUser != null)
      throw new ConflictAppException("A user with the provided email already exists.");

    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);
    
    var user = new UsersModel
    {
      Email = request.Email,
      Password = hashedPassword,
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

    var userPermissions = await _roleService.GetEffectivePermissions(user.Id);

    return new RegisterResponseDto
    {
      Id = user.Id,
      Permissions = userPermissions
    };
  }

  public async Task<bool> LogoutUserAsync(Guid userId, string? deviceIp)
  {

    var isSessionsRevoked = await _tokenService.RevokeSessionAsync(userId, deviceIp);
    if(!isSessionsRevoked)
      throw new InvalidOperationException("No active sessions");

    return true;
  }
}