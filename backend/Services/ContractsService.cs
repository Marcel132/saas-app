using Microsoft.EntityFrameworkCore;

public class ContractsService
{
  private readonly AppDbContext _context;

  public ContractsService(AppDbContext context)
  {
    _context = context;
  }

  public async Task<List<ContractDto>> GetAllContractsAsync(int? authorId = null)
  {

    var contractQuery = _context.Contracts
      .Include(c => c.Applications)
      .ThenInclude(a => a.User);
    // .ThenInclude(u => u.UserData);

    var userData = await _context.UserData.ToDictionaryAsync(u => u.UserId);

    var contracts = await contractQuery.ToListAsync()
      ?? throw new KeyNotFoundException("Cannot find any contract");

    var dto = contracts.Select(c => new ContractDto
    {
      Id = c.Id,
      AuthorId = c.AuthorId,
      Price = c.Price,
      Status = c.Status,
      Description = c.Description,
      CreatedAt = c.CreatedAt,
      UpdatedAt = c.UpdatedAt,
      Deadline = c.Deadline,
      Applications = (authorId == c.AuthorId)
        ? c.Applications.Select(a =>
        {
          userData.TryGetValue(a.UserId, out var ud);
          return new ContractApplicationDto
          {
            UserId = a.UserId,
            Email = a.User.Email,
            UserName = ud != null ? $"{ud.FirstName} {ud.LastName}" : a.User.Email,
            AppliedAt = a.AppliedAt
          };
        }).ToList()
        : new List<ContractApplicationDto>()
    }).ToList();

    return dto;
  }

  public async Task<ContractDto> GetContractByIdAsync(int contractId, int? authorId = null)
  {

    var contract = await _context.Contracts
      .Include(c => c.Applications)
      .ThenInclude(a => a.User)
      .Where(c => c.Id == contractId)
      .FirstOrDefaultAsync()
      ?? throw new KeyNotFoundException($"Contract with ID: {contractId} not found");

    var userData = await _context.UserData.ToDictionaryAsync(u => u.UserId);

    var dto = new ContractDto
    {
      Id = contract.Id,
      AuthorId = contract.AuthorId,
      Price = contract.Price,
      Status = contract.Status,
      Description = contract.Description,
      CreatedAt = contract.CreatedAt,
      UpdatedAt = contract.UpdatedAt,
      Deadline = contract.Deadline,
      Applications = (authorId == contract.AuthorId)
        ? contract.Applications.Select(a =>
        {
          userData.TryGetValue(a.UserId, out var ud);
          return new ContractApplicationDto
          {
            UserId = a.UserId,
            Email = a.User.Email,
            UserName = ud != null ? $"{ud.FirstName} {ud.LastName}" : a.User.Email,
            AppliedAt = a.AppliedAt
          };
        }).ToList()
        : new List<ContractApplicationDto>()
    };

    return dto;
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
      await _context.SaveChangesAsync();

      var createdContract = await _context.Contracts.AddAsync(ctrc);

      var userContract = new ContractDto
      {
        Id = ctrc.Id,
        AuthorId = ctrc.AuthorId,
        Price = ctrc.Price,
        Description = ctrc.Description,
        Status = ctrc.Status,
        CreatedAt = ctrc.CreatedAt,
        UpdatedAt = ctrc.UpdatedAt,
        Deadline = ctrc.Deadline
      };

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

  // public async Task<bool> TakeContractAsync(int id, int userId)
  // {
  //   if (id <= 0 || userId <= 0)
  //     throw new ArgumentException("ID must be greater than 0");

  //   try
  //   {
  //     var contract = await _context.Contracts
  //       .FirstOrDefaultAsync(c => c.Id == id)
  //       ?? throw new KeyNotFoundException($"Contract with ID {id} not found.");

  //     contract.TargetId = userId;
  //     await _context.SaveChangesAsync();
  //     return true;
  //   }
  //   catch (System.Exception)
  //   {
  //     throw;
  //   }
  // }
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

  public async Task<ContractApplicationDto> ApplyForContractAsync(int contractId, int userId)
  {
    if (contractId <= 0 || userId <= 0)
      throw new ArgumentException("Invalid contract ID or user ID");

    var isApplied = await _context.ContractApplications
        .FirstOrDefaultAsync(ca => ca.UserId == userId && ca.ContractId == contractId);
    if (isApplied != null)
      throw new ArgumentException($"User with ID: {userId} already applied for contract {contractId}");

    var user = await _context.Users
        .Where(u => u.Id == userId)
        .Select(u => new ContractApplicationDto
        {
          UserId = u.Id,
          Email = u.Email,
          UserName = (u.UserData.FirstName ?? "") + " " + (u.UserData.LastName ?? "")
        })
        .FirstOrDefaultAsync();

    if (user == null)
      throw new KeyNotFoundException($"User with ID: {userId} not found.");

    var contractApplicationData = new ContractApplicationModel
    {
      ContractId = contractId,
      UserId = userId,
      AppliedAt = DateTime.UtcNow
    };

    await _context.ContractApplications.AddAsync(contractApplicationData);
    await _context.SaveChangesAsync();

    return user;
  }

  public async Task<ContractAcceptUserDto> AcceptContractAsync(int contractId, int userId, int authorId)
  {
    using var transaction = await _context.Database.BeginTransactionAsync();

    try
    {
      var contract = await _context.Contracts
        .Where(c => c.Id == contractId)
        .FirstOrDefaultAsync()
        ?? throw new KeyNotFoundException($"Contract with ID {contractId} does not exists");

      Console.WriteLine($"AuthorID: {authorId}, Contract AuthorID: {contract.AuthorId}");

      if (contract.AuthorId != authorId)
        throw new UnauthorizedAccessException($"You are not allowed to use this method");

      contract.TargetId = userId;
      contract.Status = StatusOfContractModel.InProgress;

      await _context.SaveChangesAsync();

      var response = new ContractAcceptUserDto
      {
        UserId = userId,
        Accpeted = AcceptEnum.Accepted
      };

      await transaction.CommitAsync();
      return response;
      
    }
    catch (System.Exception)
    {
      await transaction.RollbackAsync();
      throw;
    }
  }
}
