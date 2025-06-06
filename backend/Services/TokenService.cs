using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;

public class TokenService
{

  public readonly string _jwtKey;
  public readonly string _issuer;
  public readonly string _audience;

  private readonly AppDbContext _context;

  public TokenService(IConfiguration configuration, AppDbContext context)
  {
    _context = context;
    _jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured in appsettings.json");
    _issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured in appsettings.json");
    _audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured in appsettings.json");
  }

  public async Task<string> GenerateToken(int userId, string role, string ip, string userAgent)
  {
    if (string.IsNullOrEmpty(_jwtKey) || string.IsNullOrEmpty(_issuer) || string.IsNullOrEmpty(_audience))
    {
      throw new InvalidOperationException("JWT configuration is not properly set.");
    }
    if (userId <= 0)
    {
      throw new ArgumentException("User ID must be a positive integer.", nameof(userId));
    }
    if (string.IsNullOrEmpty(role))
    {
      throw new ArgumentException("Role cannot be null or empty.", nameof(role));
    }


    var claims = new[]
    {
      new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
      new Claim(ClaimTypes.Role, role),
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
      issuer: _issuer,
      audience: _audience,
      claims: claims,
      expires: DateTime.UtcNow.AddMinutes(25),
      signingCredentials: creds
    );

    if(token == null)
    {
      throw new InvalidOperationException("Failed to create JWT token.");
    }

    var user = _context.users.FirstOrDefault(u => u.Id == userId);
    if(user == null)
    {
      throw new KeyNotFoundException($"User with ID {userId} not found.");
    }
    var tokenHash = BCrypt.Net.BCrypt.HashPassword(token.ToString());
    var session = new SessionModel
    {
      UserId = userId,
      RefreshToken = Guid.NewGuid().ToString(),
      AuthToken = tokenHash,
      CreatedAt = DateTime.UtcNow,
      ExpiresAt = DateTime.UtcNow.AddDays(7),
      UserAgent = userAgent,
      Ip = ip,
      Revoked = false
    };

    await _context.sessions.AddAsync(session);
    await _context.SaveChangesAsync();
    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}