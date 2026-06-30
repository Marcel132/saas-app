namespace backend.Application.Security;

public static class Permissions
{
  public static class Users
  {
    public const string ReadAll = "users.read-all";
    public const string Read = "users.read";
  }
  public static class Profile
  {
    public const string Read = "profile.read";
    public const string Update = "profile.update";
    public const string Delete = "profile.delete";
  }
  public static class Applications
  {
    public const string ReadOwn = "applications.read-own";
    public const string Review = "applications.review";
  }
  public static class Contracts
  {
    public const string Read = "contracts.read";
    public const string ReadOwn = "contracts.read-own";
    public const string Create = "contracts.create";
    public const string Update = "contracts.update";
    public const string ReadApplications = "contracts.read-applications";
    public const string Apply = "contracts.apply";
  }
  public static class ContractsSelf
  {
    public const string Read = "contracts-self.read";
  }
}