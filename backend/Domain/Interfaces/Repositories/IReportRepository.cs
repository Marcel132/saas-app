using backend.Domain.Entities;

namespace backend.Domain.Interfaces.Repositories;
public interface IReportRepository
{
  public Task CreateReport(ContractReport report);
}