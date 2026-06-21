namespace backend.Domain.EntitiesNew;

public class Role
{
  public Guid Id { get; private set; } = Guid.NewGuid();
  public string Code { get; private set; } = null!;
  public string Name { get; private set; } = null!;
  public bool IsActive { get; private set; } = true;

  private Role() {}
}