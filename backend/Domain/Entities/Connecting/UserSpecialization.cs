public class UserSpecialization
{
  public Guid UserId { get; private set; }
  public User User { get; private set; } = null!;

  public Specialization Specialization { get; private set; }

  private UserSpecialization() { } // EF

  public UserSpecialization(Guid userId, Specialization specialization)
  {
    UserId = userId;
    Specialization = specialization;
  }
}