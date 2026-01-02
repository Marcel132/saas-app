public class Session
{
  public int SessionId { get; private set;}
  public Guid UserId { get; private set;}

  public string RefreshTokenHash { get; private set; } = string.Empty;
  public DateTime CreatedAt { get; private set; }
  public DateTime ExpiresAt { get; private set; }
  public string UserAgent { get; private set; } = string.Empty;
  public string Ip { get; private set; } = string.Empty;
  public bool Revoked { get; private set; } = false;

  private Session() {} // EF

  public static Session Create(
    Guid userId,
    string refreshToken,
    string userAgent,
    string deviceIp
  )
  {
    return new Session
    {
      UserId = userId,
      RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken),
      CreatedAt = DateTime.UtcNow,
      ExpiresAt = DateTime.UtcNow.AddDays(7),
      UserAgent = userAgent,
      Ip = deviceIp,
    };
  }

  public void RevokeSession()
  {
    ExpiresAt = DateTime.UtcNow;
    Revoked = true;
  }

}