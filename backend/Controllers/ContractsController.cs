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


  public ContractsController(ILogger<ContractsController> logger, ContractsService contractsService)
  {
    _logger = logger;
    _contractsService = contractsService;
  }

  // [Authorize]
  [HttpGet]
  public async Task<IActionResult> GetContracts()
  {
    try
    {
      var contracts = await _contractsService.GetAllContractsAsync();

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

    try
    {
      var contract = await _contractsService.GetContractByIdAsync(id);

      return Ok(new { success = true, data = contract, message = "Contract retrieved successfully." });
    }
    catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
    catch (KeyNotFoundException ex) {  return NotFound(new { success = false, message = ex.Message }); }
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

    if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
      return Unauthorized(new { success = false, message = "Invalid or missing user ID claim." });
      
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
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateContract(int id, [FromBody] ContractRequestModel request)
  {
    ArgumentNullException.ThrowIfNull(request, "Request must have a value");
    if (id <= 0)
      return BadRequest(new { message = "Invalid contract ID." });
    if (!ModelState.IsValid)
      return BadRequest(new { message = "Invalid contract data." });

    if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
      return Unauthorized(new { success = false, message = "Invalid or missing user ID claim." });
      
    try
    {
      var isContractUpdated = await _contractsService.UpdateContractAsync(id, request, userId);

      if (isContractUpdated == false)
        return NotFound(new { message = $"Contract with ID {id} not found." });

      return Ok(new { success = true, message = "Contract updated successfully." });
    }
    catch (ArgumentException ex)  { return BadRequest(new { message = ex.Message }); }
    catch (KeyNotFoundException ex) {  return NotFound(new { message = ex.Message });  }
    catch (UnauthorizedAccessException ex) {return Unauthorized(new { message = ex.Message }); }
    catch (System.Exception)
    {
      return StatusCode(500, new { success = false, message = "An error occurred while updating a contract" });
    }

  }

  [Authorize(Roles = "Admin, Company")]
  [HttpDelete("{id}")]
  public IActionResult DeleteContract(int id)
  {
    return Ok(new { message = "Delete contract endpoint is under construction." });
  }

}