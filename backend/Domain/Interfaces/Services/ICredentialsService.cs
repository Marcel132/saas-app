public interface ICredentialsService
{
  public Task<CredentialsDto> GenerateCredentials(Guid userId);
}