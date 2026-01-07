public class Role
{
  public Guid RoleId {get; private set;}
  public string Code {get; private set;} = string.Empty;
  public string Name {get; private set;} = string.Empty;
  public bool IsActive {get; private set;}
  private Role() {}
  
}

