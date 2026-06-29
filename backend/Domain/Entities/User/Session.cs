using backend.Infrastructure.Security;

namespace backend.Domain.Entities;

public class Session
{
  public long Id { get; private set; }
  public Guid UserId { get; private set; }
  public string RefreshTokenHash { get; private set; } = string.Empty;
  public DateTime CreatedAt { get; private set; }
  public DateTime ExpiresAt { get; private set; }
  public string IpAddress { get; private set; } = string.Empty;
  public string UserAgent { get; private set; } = string.Empty;
  public bool Revoked { get; private set; }
  public bool Used { get; private set; }
  public long? ReplacedByTokenId { get; private set; }

  private Session() { }

  public User User { get; private set; } = null!;

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
      Revoked = false,
      Used = false,
      CreatedAt = DateTime.UtcNow,
      ExpiresAt = DateTime.UtcNow.AddDays(7),
      IpAddress = ipAddress,
      UserAgent = userAgent,
    };
  }

  public void RevokeSession(long? replacedByTokenId)
  {
    Revoked = true;
    Used = true;
    if (replacedByTokenId.HasValue)
    {
      ReplacedByTokenId = replacedByTokenId.Value;
    }
  }

  public void ReplaceByTokenId(long sessionId)
  {
    if (sessionId <= 0)
      throw new ValueOutOfRangeAppException();

    if (ReplacedByTokenId != null)
      throw new InvalidOperationAppException();

    ReplacedByTokenId = sessionId;
  }

}