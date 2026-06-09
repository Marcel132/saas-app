public interface IMeQueryRepository
{
  public Task<List<ApplicationDto>> GetApplications(Guid userId);
}