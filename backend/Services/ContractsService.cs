using Microsoft.EntityFrameworkCore;

public class ContractsService
{
  private readonly AppDbContext _context;

  public ContractsService(AppDbContext context)
  {
    _context = context;
  }

  public async Task<List<ContractDto>> GetAllContractsAsync()
  {
    return await _context.Contracts
      .Include(c => c.Author)
      .Where(c => c.Author != null && DateTime.UtcNow <= (c.Deadline ?? DateTime.MaxValue))
      .Select(c => new ContractDto
      {
        Id = c.Id,
        AuthorId = c.AuthorId,
        Price = c.Price,
        Status = c.Status,
        Description = c.Description,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt,
        Deadline = c.Deadline
      })
      .ToListAsync();
  }

  public async Task<ContractDto> GetContractByIdAsync(int contractId)
  {
    var contract = await _context.Contracts
      .Include(c => c.Author)
      .Where(c => c.Id == contractId)
      .Select(c => new ContractDto
      {
        Id = c.Id,
        AuthorId = c.AuthorId,
        Price = c.Price,
        Status = c.Status,
        Description = c.Description,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt,
        Deadline = c.Deadline
      })
      .FirstOrDefaultAsync()
      ?? throw new KeyNotFoundException($"Contract with ID {contractId} not found.");

    return contract;
  }

  public async Task<ContractDto> CreateContractAsync(ContractModel contract, int userId)
  {
    if (contract == null)
      throw new ArgumentNullException(nameof(contract), "Contract must have a value.");
    if (userId <= 0)
      throw new ArgumentException("UserId must have a value");
    if (contract.Price <= 0)
      throw new ArgumentException("Price must be greater than zero.");

    if (string.IsNullOrWhiteSpace(contract.Description))
      throw new ArgumentNullException(nameof(contract.Description), "A contract must have a description");

    try
    {

      var ctrc = new ContractModel
      {
        AuthorId = userId,
        Price = contract.Price,
        Description = contract.Description,
        Status = contract.Status,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        Deadline = contract.Deadline ?? DateTime.UtcNow.AddDays(30)
      };

      var createdContract = await _context.Contracts.AddAsync(ctrc);

      var userContract = new ContractDto
      {
        AuthorId = ctrc.AuthorId,
        Price = ctrc.Price,
        Description = ctrc.Description,
        Status = ctrc.Status,
        CreatedAt = ctrc.CreatedAt,
        UpdatedAt = ctrc.UpdatedAt,
        Deadline = ctrc.Deadline
      };

      await _context.SaveChangesAsync();
      return userContract;
    }
    catch
    {
      throw;
    }
  }

  public async Task<bool> UpdateContractAsync(int id, ContractRequestModel request, int userId)
  {
    if (request == null)
      throw new ArgumentNullException(nameof(request), "Request must have a value.");

    try
    {

      var contract = await _context.Contracts
        .FirstOrDefaultAsync(c => c.Id == id)
        ?? throw new KeyNotFoundException($"Contract with ID {id} not found.");

      if (contract.AuthorId != userId)
        throw new UnauthorizedAccessException("You are not authorized to update this contract.");

      contract.Price = request.Price > 0 ? request.Price : contract.Price;
      contract.Status = Enum.IsDefined(typeof(StatusOfContractModel), request.Status) ? request.Status : contract.Status;
      contract.Description = !string.IsNullOrWhiteSpace(request.Description) ? request.Description : contract.Description;
      contract.Deadline = request.Deadline ?? contract.Deadline;

      await _context.SaveChangesAsync();

      return true;
    }
    catch (System.Exception)
    {
      throw;
    }

  }

  public async Task<bool> DeleteContractAsync(int contractId, int userId)
  {
    if (contractId <= 0)
      throw new ArgumentOutOfRangeException("Contract ID is lower than 1");
    if (userId <= 0)
      throw new ArgumentOutOfRangeException("User ID is lower than 1");

    var contract = await _context.Contracts
      .FirstOrDefaultAsync(c => c.Id == contractId)
      ?? throw new KeyNotFoundException($"Not found contract with ID: {contractId}");

    if (contract.AuthorId != userId)
      throw new UnauthorizedAccessException("You are not allowed to delete this contract");

    _context.Contracts.Remove(contract);
    await _context.SaveChangesAsync();

    return true;
  }
}
