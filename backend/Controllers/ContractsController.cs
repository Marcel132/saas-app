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
      if (contracts == null || contracts.Count == 0)
      {
        return NotFound(new { success = false, message = "No contracts found." });
      }
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
    {
      return BadRequest(new { success = false, message = "Invalid contract ID." });
    }

    try
    {
      var contract = await _contractsService.GetContractByIdAsync(id);
      if (contract == null)
      {
        return NotFound(new { success = false, message = $"Contract with ID {id} not found." });
      }

      return Ok(new { success = true, data = contract, message = "Contract retrieved successfully." });
    }
    catch (KeyNotFoundException ex)
    {
      return NotFound(new { success = false, message = ex.Message });
    }
    catch (System.Exception)
    {
      return StatusCode(500, new { success = false, message = "An error occurred while retrieving the contract." });
    }
  }

  // [Authorize(Roles = "Admin, Company")]
  [HttpPost]
  public async Task<IActionResult> CreateContract([FromBody] ContractModel request)
  {
    ArgumentNullException.ThrowIfNull(request);
    // if (request == null)
    // {
    //   return BadRequest(new { success = false, message = "Request body is null." });
    // }

    if (!ModelState.IsValid)
    {
      return BadRequest(new { success = false, message = "Invalid contract data."});
    }

    try
    {
      var createdContract = await _contractsService.CreateContractAsync(request);
      if (createdContract == null)
      {
        return BadRequest(new { success = false, message = "Failed to create contract." });
      }
      return Ok( new { success = true, data = createdContract, message = "Contract created successfully." });
    }
    catch (ArgumentException ex)
    {
      return BadRequest(new { success = false, message = ex.Message });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error creating contract");
      return StatusCode(500, new { success = false, message = "An error occurred while creating the contract." });
    }
  }

  [Authorize(Roles = "Admin, Company")]
  [HttpPut("{id}")]
  public IActionResult UpdateContract(int id, [FromBody] object contractData)
  {
    return Ok(new { message = "Update contract endpoint is under construction." });
  }

  [Authorize(Roles = "Admin, Company")]
  [HttpDelete("{id}")]
  public IActionResult DeleteContract(int id)
  {
    return Ok(new { message = "Delete contract endpoint is under construction." });
  }

}