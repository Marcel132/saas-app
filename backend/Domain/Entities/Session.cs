public class Session
{
  public int SessionId { get; private set;}
  public Guid UserId { get; private set;}
  public string RefreshTokenHash { get; private set; } = string.Empty;
  public DateTime CreatedAt { get; private set; }
  public DateTime ExpiresAt { get; private set; }
  public string IpAddress { get; private set; } = string.Empty;
  public string UserAgent { get; private set; } = string.Empty;
  public bool Revoked { get; private set; } = false;
  public bool Used { get; private set; } = false;
  public int? ReplacedByTokenId { get; private set; } = null;

  private Session() {} // EF

  public static Session Create(
    Guid userId,
    string refreshToken,
    string userAgent,
    string ipAddress
  )
  {
    return new Session
    {
      UserId = userId,
      RefreshTokenHash = TokenHasher.HashToken(refreshToken),
      CreatedAt = DateTime.UtcNow,
      ExpiresAt = DateTime.UtcNow.AddDays(7),
      IpAddress = ipAddress,
      UserAgent = userAgent,
    };
  }
  
  public void RevokeSession(int? replacedByTokenId)
  {
    Revoked = true;
    Used = true;
    if(replacedByTokenId.HasValue)
    {
      ReplacedByTokenId = replacedByTokenId.Value;
    }
  }

  public void ReplaceByTokenId(int sessionId)
  {
    if (sessionId <= 0)
      throw new ValueOutOfRangeAppException();
    
    if(ReplaceByTokenId != null)
      throw new InvalidOperationAppException();
      
    ReplacedByTokenId = sessionId;
  }

  
}