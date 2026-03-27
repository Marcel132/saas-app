public class Session
{
  public int SessionId { get; private set;}
  public Guid UserId { get; private set;}

  public string RefreshTokenHash { get; private set; } = string.Empty;
  public DateTime CreatedAt { get; private set; }
  public DateTime ExpiresAt { get; private set; }
  public string Ip { get; private set; } = string.Empty;
  public string UserAgent { get; private set; } = string.Empty;
  public bool Revoked { get; private set; } = false;
  public bool Used { get; private set; } = false;
  public int? ReplacedByTokenId { get; private set; }

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
      RefreshTokenHash = TokenHasher.HashToken(refreshToken),
      CreatedAt = DateTime.UtcNow,
      ExpiresAt = DateTime.UtcNow.AddDays(5),
      Ip = deviceIp,
      UserAgent = userAgent,
    };
  }
  public void RevokeSession(int? replacedByTokenId)
  {
    ExpiresAt = DateTime.UtcNow;
    Revoked = true;
    Used = true;
    if(replacedByTokenId != null)
    {
      ReplacedByTokenId = replacedByTokenId;
    }
  }

  public void MarkAsUsed()
  {
    Used = true;
  }
}