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
  private readonly SymmetricSecurityKey _signingKey;
  private readonly TokenValidationParameters _validationParameters;
  private readonly JwtSecurityTokenHandler _tokenHandler = new();

  public TokenService(IConfiguration configuration, AppDbContext context)
  {
    _jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured in appsettings.json");
    _issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured in appsettings.json");
    _audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured in appsettings.json");

    _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
    _validationParameters = new TokenValidationParameters
    {
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateLifetime = true,
      ValidateIssuerSigningKey = true,
      ValidIssuer = _issuer,
      ValidAudience = _audience,
      IssuerSigningKey = _signingKey
    };
  }
  public string GenerateAuthToken(
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
      expires: DateTime.UtcNow.AddSeconds(15),
      signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  public string GenerateRefreshToken()
  {
    var refreshTokenBytes = RandomNumberGenerator.GetBytes(64);
    return Convert.ToBase64String(refreshTokenBytes);
  }

  public ClaimsPrincipal? ValidateAuthToken(string authToken)
  {
    if (string.IsNullOrWhiteSpace(authToken) || !_tokenHandler.CanReadToken(authToken))
      return null;

    try
    {
      var principal = _tokenHandler.ValidateToken(authToken, _validationParameters, out _);

      return principal;
    }
    catch
    {
      return null;
    }
  }

}
