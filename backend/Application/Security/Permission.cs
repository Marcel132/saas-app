public static class Permissions
{
  public class Profile
  {
    public const string Read = "profile.read";
    public const string Update = "profile.update";
    public const string Delete = "profile.delete";
  }
  public class ProfileContracts
  {
    public const string Read = "profile-contracts.read";
  }
  public class ProfileApplications
  {
    public const string Read = "profile-applications.read";
    public const string Create = "profile-applications.create";
    public const string Cancel = "profile-applications.cancel";
  }
  public class Users
  {
    public const string Read = "users.read";
    public const string ReadAll = "users.read-all";
  }

  public class Contracts
  {
    public const string Create = "contracts.create";
  }
  public class ContractsSelf
  {
    public const string ModifyDetails = "contracts-self.modify-details";
    public const string Status = "contracts-self.status";
    public const string Read = "contracts-self.read";
    public const string AuthorizeApplications = "contracts-self.authorize-applications";
  }
}