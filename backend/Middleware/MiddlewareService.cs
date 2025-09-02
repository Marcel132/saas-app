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

  public async Task<ClaimsPrincipal> isValidJwtConfiguration(string token)
  {
    if (string.IsNullOrEmpty(_jwtKey) || string.IsNullOrEmpty(_issuer) || string.IsNullOrEmpty(_audience))
    {
      throw new InvalidOperationException("JWT configuration is not properly set.");
    }

    if (string.IsNullOrEmpty(token))
    {
      throw new ArgumentException("Token cannot be null or empty.", nameof(token));
    }

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

      var sessions = await _context.Sessions.ToListAsync();
      var userSession = sessions.FirstOrDefault(s => BCrypt.Net.BCrypt.Verify(token, s.AuthToken));

      if (userSession?.ExpiresAt < DateTime.UtcNow)
      {
        throw new SecurityTokenException("Invalid token: Token has expired.");
      }
      else if (userSession.Revoked)
      {
        throw new SecurityTokenException("Invalid token: Token has been revoked.");
      }
      else
      {
        if (userSession == null)
        {
          throw new SecurityTokenException("Invalid token: Session not found.");
        }
        return principal;
      }


    }
    catch (SecurityTokenException ex)
    {
      Console.WriteLine($"Security token validation failed: {ex.Message}");
      return null;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"An error occurred while validating the JWT token: {ex.Message}");
      return null;
    }
  }
}