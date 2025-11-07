[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class RequiredRoleAttribute : Attribute
{
    public string Role { get; }

    public RequiredRoleAttribute(string role)
    {
        Role = role;
    }
}