using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Cryptography;

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
  public async Task<ResponseTokenDto> GenerateAuthToken(Guid userId, string ip, string userAgent)
  {
    // Vaidate configuration and inputs
    if (string.IsNullOrEmpty(_jwtKey) || string.IsNullOrEmpty(_issuer) || string.IsNullOrEmpty(_audience))
      throw new InvalidOperationException("JWT configuration is not properly set.");

    if(userId == Guid.Empty)
      throw new ArgumentException("Invalid user ID");


    // Stat a transaction to ensure atomicity
    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
      // Verify that the user exists
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId)
        ?? throw new KeyNotFoundException($"User with ID {userId} not found.");

      // Create claims for the token
      var claims = new[]
      {
        new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
      };

      // Create signing credentials
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      // Create the JWT token
      var authToken = new JwtSecurityToken
      (
        issuer: _issuer,
        audience: _audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(15),
        signingCredentials: creds
      ) ?? throw new InvalidOperationException("Failed to create JWT token.");

      // Serialize and hash tokens
      var authTokenJwtString = new JwtSecurityTokenHandler().WriteToken(authToken);
      var authTokenHashed = TokenHasher.HashToken(authTokenJwtString);

      var refreshToken = await GenerateRefreshToken();
      var refreshTokenHashed = TokenHasher.HashToken(refreshToken);

      var session = new SessionModel
      {
        UserId = userId,
        RefreshToken = refreshTokenHashed,
        AuthToken = authTokenHashed,
        CreatedAt = DateTime.UtcNow,
        ExpiresAt = DateTime.UtcNow.AddDays(7),
        UserAgent = userAgent,
        Ip = ip,
        Revoked = false
      };

      // Save the session to the database
      await _context.Sessions.AddAsync(session);
      await _context.SaveChangesAsync();
      await transaction.CommitAsync();

      return new ResponseTokenDto
      {
        AuthToken = authTokenJwtString,
        RefreshToken = refreshToken
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
    var refreshTokenBytes = RandomNumberGenerator.GetBytes(64);
    var refreshToken = Convert.ToBase64String(refreshTokenBytes);
    return Task.FromResult(refreshToken);
  }

  public async Task<ResponseTokenDto> RefreshTokenAsync(string refreshToken)
  {
    if (string.IsNullOrEmpty(refreshToken))
      throw new ArgumentException("Refresh token is missing");

    var session = await _context.Sessions
      .Include(u => u.User)
      .Where(s => !s.Revoked && s.ExpiresAt > DateTime.UtcNow)
      .FirstOrDefaultAsync(s => s.RefreshToken == TokenHasher.HashToken(refreshToken))
      ?? throw new UnauthorizedAccessException("Session is missing");

      // Generujemy nowy access token, ale NIE nową sesję
    var claims = new[]
    {
      new Claim(JwtRegisteredClaimNames.Sub, session.UserId.ToString()),
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
      new Claim(ClaimTypes.NameIdentifier, session.UserId.ToString())
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var authToken = new JwtSecurityToken(
      issuer: _issuer,
      audience: _audience,
      claims: claims,
      expires: DateTime.UtcNow.AddMinutes(15),
      signingCredentials: creds
    );

    var authTokenJwtString = new JwtSecurityTokenHandler().WriteToken(authToken);
    var authTokenHashed = TokenHasher.HashToken(authTokenJwtString);
    
    var refreshTokenNew = await GenerateRefreshToken();
    var refreshTokenHashed = TokenHasher.HashToken(refreshTokenNew);

    session.AuthToken = authTokenHashed;
    session.RefreshToken = refreshTokenHashed;
    session.ExpiresAt = DateTime.UtcNow.AddDays(7);

    await _context.SaveChangesAsync();

    return new ResponseTokenDto
    {
      AuthToken = authTokenJwtString,
      RefreshToken = refreshTokenNew
    };
  }

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

  public async Task<bool> RevokeSessionAsync(Guid userId, string deviceIp)
  {
    var sessions = await _context.Sessions
      .Where(s => s.UserId == userId && !s.Revoked && s.Ip == deviceIp)
      .ToListAsync();

    if (sessions.Count == 0)
      return false;

    foreach (var sess in sessions)
    {
      sess.Revoked = true;
    }

    await _context.SaveChangesAsync();

    return true;
  }

}



public class TokenHasher
{
    public static string HashToken(string token)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));

            StringBuilder sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
