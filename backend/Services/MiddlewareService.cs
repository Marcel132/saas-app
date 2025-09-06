using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

public class MiddlewareService
{
  private readonly string _jwtKey;
  private readonly string _issuer;
  private readonly string _audience;
  private readonly AppDbContext _context;

  public MiddlewareService(IConfiguration configuration, AppDbContext context)
  {
    _context = context;
    _jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured in appsettings.json");
    _issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured in appsettings.json");
    _audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured in appsettings.json");
  }

  public async Task<ClaimsPrincipal> IsValidJwtConfiguration(string token)
  {
    ArgumentNullException.ThrowIfNull(token);
    ArgumentNullException.ThrowIfNull(_jwtKey);
    ArgumentNullException.ThrowIfNull(_issuer);
    ArgumentNullException.ThrowIfNull(_audience);

    try
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var validationParameter = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = _issuer,
        ValidAudience = _audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey))
      };

      var principal = tokenHandler.ValidateToken(token, validationParameter, out SecurityToken validatedToken);

      var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (string.IsNullOrEmpty(userId))
        throw new SecurityTokenException("Invalid token: Missing user ID.");

      var userSessions = await _context.Sessions
        .Where(s => s.UserId == int.Parse(userId) && !s.Revoked)
        .ToListAsync();

      var userSession = userSessions.FirstOrDefault(s => BCrypt.Net.BCrypt.Verify(token, s.AuthToken))
        ?? throw new SecurityTokenException("Invalid token: Session not found.");

      if (userSession.ExpiresAt < DateTime.UtcNow)
        throw new SecurityTokenException("Invalid token: Token has expired.");

      if (userSession.Revoked)
        throw new SecurityTokenException("Invalid token: Token has been revoked.");

      return principal;

    }
    catch (SecurityTokenException ex)
    {
      throw new SecurityTokenException($"Token validation failed: {ex.Message}", ex);
    }
    catch (Exception ex)
    {
      throw new Exception("An error occurred during token validation.", ex);
    }
  }
}