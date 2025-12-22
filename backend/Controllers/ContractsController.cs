using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class ContractsController : ControllerBase
{
  private readonly ILogger<ContractsController> _logger;
  private readonly ContractsService _contractsService;
  protected int? GetUserId()
  {
    var user = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
      ? userId
      : (int?)null;
    _logger.LogInformation($"{user}");
    return user;
  }

  public ContractsController(ILogger<ContractsController> logger, ContractsService contractsService)
  {
    _logger = logger;
    _contractsService = contractsService;
  }

  // [Authorize]
  [HttpGet]
  public async Task<IActionResult> GetContracts()
  {
    // Move to [Authorize]
    var userId = GetUserId();
    if (userId is null || userId <= 0)
      return Unauthorized(new { success = false, message = "You are not allowed to use this method" });

    try
      {
        var contracts = await _contractsService.GetAllContractsAsync(userId);

        if (contracts.Count == 0)
          return NotFound(new { success = false, message = "No contracts found." });

        return Ok(new { success = true, data = contracts, message = "Contracts retrieved successfully." });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error retrieving contracts");
        return StatusCode(500, new { success = false, message = "An error occurred while retrieving contracts." });
      }
  }

  // [Authorize]
  [HttpGet("{id}")]
  public async Task<IActionResult> GetContractById(int id)
  {
    if (id <= 0)
      return BadRequest(new { success = false, message = "Invalid contract ID." });

    var userId = GetUserId() ?? 0;
    if (userId <= 0)
      return Unauthorized(new { success = false, message = "You are not allowed to use this method" });

    try
    {
      var contract = await _contractsService.GetContractByIdAsync(id, userId);

      return Ok(new { success = true, data = contract, message = "Contract retrieved successfully." });
    }
    catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
    catch (KeyNotFoundException ex) { return NotFound(new { success = false, message = ex.Message }); }
    catch (System.Exception)
    {
      return StatusCode(500, new { success = false, message = "An error occurred while retrieving the contract." });
    }
  }

  // [Authorize(Roles = "Admin, Company")]
  [HttpPost]
  public async Task<IActionResult> CreateContract([FromBody] ContractModel request)
  {
    ArgumentNullException.ThrowIfNull(request, "Request cannot be null");

    if (!ModelState.IsValid)
      return BadRequest(new { success = false, message = "Invalid contract data." });

    var userId = GetUserId() ?? 0;
    if (userId <= 0)
      return Unauthorized(new { success = false, message = "You are not allowed to use this method" });

    try
    {

      var createdContract = await _contractsService.CreateContractAsync(request, userId);
      ArgumentNullException.ThrowIfNull(createdContract, "Failed to create a contract");

      return Ok(new { success = true, data = createdContract, message = "Contract created successfully." });
    }
    catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error creating contract");
      return StatusCode(500, new { success = false, message = "An error occurred while creating the contract." });
    }
  }

  // [Authorize(Roles = "Admin, Company")]
  // [HttpPut("{id}")]
  // public async Task<IActionResult> UpdateContract(int id, [FromBody] ContractRequestModel request)
  // {
  //   ArgumentNullException.ThrowIfNull(request, "Request must have a value");
  //   if (id <= 0)
  //     return BadRequest(new { message = "Invalid contract ID." });
  //   if (!ModelState.IsValid)
  //     return BadRequest(new { message = "Invalid contract data." });

  //   var userId = GetUserId() ?? 0;
  //   if (userId <= 0)
  //     return Unauthorized(new { success = false, message = "You are not allowed to use this method" });

  //   try
  //   {
  //     var isContractUpdated = await _contractsService.UpdateContractAsync(id, request, userId);

  //     if (isContractUpdated == false)
  //       return NotFound(new { message = $"Contract with ID {id} not found." });

  //     return Ok(new { success = true, message = "Contract updated successfully." });
  //   }
  //   catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
  //   catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
  //   catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
  //   catch (System.Exception)
  //   {
  //     return StatusCode(500, new { success = false, message = "An error occurred while updating a contract" });
  //   }

  // }

  // [Authorize(Roles = "Admin, Company")]
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteContract(int id)
  {
    var userId = GetUserId() ?? 0;
    if (userId <= 0)
      return Unauthorized(new { success = false, message = "You are not allowed to use this method" });

    try
    {
      await _contractsService.DeleteContractAsync(id, userId);

      return Ok(new { success = true, message = $"Deleted contract with ID: {id}" });
    }
    catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
    catch (KeyNotFoundException ex) { return NotFound(new { success = false, message = ex.Message }); }
    catch (UnauthorizedAccessException ex) { return Unauthorized(new { success = false, message = ex.Message }); }
    catch (System.Exception)
    {
      return StatusCode(500, new { success = false, message = "An error occurred while deleteing a contract" });
    }

  }

  [HttpPost("{id}/apply")]
  public async Task<IActionResult> ApplyForContract(int id)
  {
    var userId = GetUserId() ?? 0;
    if (userId <= 0)
      return Unauthorized(new { success = false, message = "You are not allowed to use this method" });

    if (id <= 0)
      return BadRequest(new { success = false, message = "Invalid contract ID" });

    try
    {
      var appliedForContract = await _contractsService.ApplyForContractAsync(id, userId);

      return Ok(new { success = true, data = appliedForContract, message = "Applied successfuly" });
    }
    catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
    catch (KeyNotFoundException ex) { return BadRequest(new { success = false, message = ex.Message }); }
    catch (System.Exception)
    {
      return StatusCode(500, new { success = false, message = "An error occurred while applying for contract" });
    }
  }

  [HttpPut("{contractId}/accept/{userId}")]
  public async Task<IActionResult> AcceptContract(int contractId, int userId)
  {
    if (contractId <= 0)
      return BadRequest(new { success = false, message = "Invalid contract ID" });
    if (userId <= 0)
      return BadRequest(new { success = false, message = "Invalid user ID" });

    var authorId = GetUserId() ?? 0;
    if (authorId <= 0)
      return Unauthorized(new { success = false, message = "You are not allowed to use this method" });
      
    try
    {
      var accepted = await _contractsService.AcceptContractAsync(contractId, userId, authorId);
      _logger.LogInformation(accepted.ToString());

      return Ok(new { success = true, data = accepted, message = "Contract accepted succesfuly" });
    }
    catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
    catch (KeyNotFoundException ex) { return BadRequest(new { success = false, message = ex.Message }); }
    catch (UnauthorizedAccessException ex) { return BadRequest(new { success = false, message = ex.Message }); }
    catch (System.Exception)
    {
      return StatusCode(500, new { success = false, message = "An error occurred while accepting for contract" });
    }
  }

}