using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using backend.Api.Controllers.Auth.DTOs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace backend.Application.Services;

public class TokenService
{
  private readonly JwtOptions _jwtOptions;
  private readonly SymmetricSecurityKey _signingKey;
  private readonly TokenValidationParameters _validationParameters;
  private readonly JwtSecurityTokenHandler _tokenHandler = new();

  private const string PermissionClaim = "permission";

  public TokenService(IOptions<JwtOptions> options)
  {
    _jwtOptions = options.Value;

    if (string.IsNullOrWhiteSpace(_jwtOptions.Key) || string.IsNullOrWhiteSpace(_jwtOptions.Audience) || string.IsNullOrWhiteSpace(_jwtOptions.Issuer))
      throw new InvalidOperationAppException("JWT options not configured");


    _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
    _validationParameters = new TokenValidationParameters
    {
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateLifetime = true,
      ValidateIssuerSigningKey = true,
      ValidIssuer = _jwtOptions.Issuer,
      ValidAudience = _jwtOptions.Audience,
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

    foreach (var perm in permissions)
      claims.Add(new Claim(PermissionClaim, perm));

    var creds = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
      issuer: _jwtOptions.Issuer,
      audience: _jwtOptions.Audience,
      claims: claims,
      expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenLifetimeInMinutes),
      signingCredentials: creds
    );

    return _tokenHandler.WriteToken(token);
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
