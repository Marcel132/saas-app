using Microsoft.EntityFrameworkCore;

public class ContractsService
{
  private readonly AppDbContext _context;

  public ContractsService(AppDbContext context)
  {
    _context = context;
  }

  public async Task<List<ContractModel>> GetAllContractsAsync()
  {
    try
    {
      return await _context.Contracts.Include(c => c.Author)
        .Where(c => c.Author != null && DateTime.UtcNow <= (c.Deadline ?? DateTime.MaxValue))
        .ToListAsync();
    }
    catch (Exception ex)
    {
      throw new Exception("An error occurred while retrieving contracts.", ex);
    }
  }
  public async Task<ContractModel> GetContractByIdAsync(int contractId)
  {
    var contract = await _context.Contracts
      .Include(c => c.Author)
      .FirstOrDefaultAsync(c => c.Id == contractId)
      ?? throw new KeyNotFoundException($"Contract with ID {contractId} not found.");

    return contract;
  }

  public async Task<ContractModel> CreateContractAsync(ContractModel contract, string refreshToken)
  {
    ArgumentNullException.ThrowIfNull(contract);

    var session = await _context.Sessions
      .Include(s => s.User)
      .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken && !s.Revoked)
      ?? throw new UnauthorizedAccessException("Invalid or revoked refresh token.");

    if (session.UserId <= 0 || session.User == null)
      throw new ArgumentException($"Author with ID {session.UserId} does not exist.");
    if (contract.Price <= 0)
      throw new ArgumentException("Price must be greater than zero.");
    ArgumentException.ThrowIfNullOrEmpty(contract.Description);

    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {

      var ctrc = new ContractModel
      {
        AuthorId = session.UserId,
        Price = contract.Price,
        Description = contract.Description,
        Status = contract.Status,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        Deadline = DateTime.UtcNow.AddDays(30)
      };

      var createdContract = await _context.Contracts.AddAsync(ctrc);
      await _context.SaveChangesAsync();
      await transaction.CommitAsync();
      return contract;
    }
    catch
    {
      await transaction.RollbackAsync();
      throw;
    }
  }

  public async Task<bool> UpdateContractAsync(int id, ContractRequestModel request, string refreshToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {

      var session = await _context.Sessions
      .Include(s => s.User)
      .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken && !s.Revoked)
      ?? throw new UnauthorizedAccessException("Invalid or revoked refresh token.");

      var userId = session.UserId;

      var contract = await _context.Contracts
        .FirstOrDefaultAsync(c => c.Id == id)
        ?? throw new KeyNotFoundException($"Contract with ID {id} not found.");

      if (contract.AuthorId != session.UserId)
        throw new UnauthorizedAccessException("You are not authorized to update this contract.");

      contract.Price = request.Price > 0 ? request.Price : contract.Price;
      contract.Status = Enum.IsDefined(typeof(StatusOfContractModel), request.Status) ? request.Status : contract.Status;
      contract.Description = !string.IsNullOrWhiteSpace(request.Description) ? request.Description : contract.Description;
      contract.Deadline = request.Deadline ?? contract.Deadline;

      await _context.SaveChangesAsync();
      await transaction.CommitAsync();

      return true;
    }
    catch (System.Exception)
    {
      await transaction.RollbackAsync();
      throw;
    }

  }
}