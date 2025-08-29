using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

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

  // Generates a JWT auth token for a user and creates a session record in the database.
  // The token includes claims for the user's ID and role, and is signed using a symmetric
  public async Task<ResponseTokenModel> GenerateAuthToken(int userId, string role, string ip, string userAgent)
  {
    // Vaidate configuration and inputs
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

    // Stat a transaction to ensure atomicity
    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
      // Verify that the user exists
      var user = await _context.users.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new KeyNotFoundException($"User with ID {userId} not found.");

      // Create claims for the token
      var claims = new[]
      {
        new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
        new Claim(ClaimTypes.Role, role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
      };

      // Create signing credentials
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      // Create the JWT token
      var token = new JwtSecurityToken
      (
        issuer: _issuer,
        audience: _audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(15),
        signingCredentials: creds
      ) ?? throw new InvalidOperationException("Failed to create JWT token.");

      // Serialize and hash the token
      var jwtString = new JwtSecurityTokenHandler().WriteToken(token);
      var tokenHash = BCrypt.Net.BCrypt.HashPassword(jwtString);
      var session = new SessionModel
      {
        UserId = userId,
        RefreshToken = await GenerateRefreshToken(),
        AuthToken = tokenHash,
        CreatedAt = DateTime.UtcNow,
        ExpiresAt = DateTime.UtcNow.AddDays(7),
        UserAgent = userAgent,
        Ip = ip,
        Revoked = false
      };

      // Save the session to the database
      await _context.sessions.AddAsync(session);
      await _context.SaveChangesAsync();
      await transaction.CommitAsync();
      
      return new ResponseTokenModel
      {
        AuthToken = jwtString,
        RefreshToken = session.RefreshToken
      };
    }
    catch (Exception)
    {
      await transaction.RollbackAsync();
      throw;
    }
  }

  public Task<string> GenerateRefreshToken()
  {
    var refreshToken = Guid.NewGuid().ToString();
    return Task.FromResult(refreshToken);
  }

}

public class ResponseTokenModel
{
  public string AuthToken { get; set; } = string.Empty;
  public string RefreshToken { get; set; } = string.Empty;
}