using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

public class TokenService
{

  public readonly string _jwtKey;
  public readonly string _issuer;
  public readonly string _audience;

  public TokenService(IConfiguration configuration, AppDbContext context)
  {
    _jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured in appsettings.json");
    _issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured in appsettings.json");
    _audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured in appsettings.json");
  }
public ResponseTokenDto GenerateAuthToken(
  Guid userId,
  IEnumerable<string> permissions)
{
  var claims = new List<Claim>
  {
    new(JwtRegisteredClaimNames.Sub, userId.ToString()),
    new(ClaimTypes.NameIdentifier, userId.ToString()),
    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
  };

  foreach (var permission in permissions)
    claims.Add(new Claim("permission", permission));

  var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
  var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

  var token = new JwtSecurityToken(
    issuer: _issuer,
    audience: _audience,
    claims: claims,
    expires: DateTime.UtcNow.AddMinutes(15),
    signingCredentials: creds
  );

  return new ResponseTokenDto
  {
    AuthToken = new JwtSecurityTokenHandler().WriteToken(token),
    RefreshToken = GenerateRefreshToken()
  };
}

  public string GenerateRefreshToken()
  {
    var refreshTokenBytes = RandomNumberGenerator.GetBytes(64);
    return Convert.ToBase64String(refreshTokenBytes);
  }

  // TODO: Refactor!!!
  public ClaimsPrincipal? ValidateAuthToken(string authToken)
  {
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(_jwtKey);

    try
    {
      var principal = tokenHandler.ValidateToken(authToken, new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = _issuer,
        ValidAudience = _audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
      }, out _);

      return principal;
    }
    catch
    {
      return null;
    }
  }

}