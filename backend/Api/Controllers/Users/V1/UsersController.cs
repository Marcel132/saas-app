using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
  private readonly UserCommandService _commandService;
  private readonly UserQueryService _queryService;

  public UsersController(
    UserCommandService commandService, 
    UserQueryService queryService
  )
  {
    _commandService = commandService;
    _queryService = queryService;
  }

  [HasPermission(Permissions.Users.ReadAll)]
  [HttpGet]
  public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
  {
    var users = await _queryService.GetAllAsync(page, pageSize, search);
    
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      "Users retrieved successfully", 
      DomainErrorCodes.AuthCodes.Success,
      users
      ));
  }

  [HasPermission(Permissions.Users.Read)]
  [HttpGet("{userId}")]
  public async Task<IActionResult> GetUserById([FromRoute] Guid userId)
  {
    var currentUserId = UserContextExtension.GetUserId(User);

    var user = await _queryService.GetUserByIdAsync(userId, currentUserId);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      "User retrieved successfully",
      DomainErrorCodes.AuthCodes.Success,
      user
    ));
  }


  [HasPermission(Permissions.Profile.Read)]
  [HttpGet("me")]
  public async Task<IActionResult> GetCurrentUser()
  {
    var userId = UserContextExtension.GetUserId(User);
    var user = await _queryService.GetCurrentUserByIdAsync(userId);
    
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      "Current user retrieved successfully", 
      DomainErrorCodes.AuthCodes.Success,
      user
      ));
  }

  // TODO: REFACTOR 
  [HasPermission(Permissions.Profile.Update)]
  [HttpPut("me")]
  public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserDto request)
  {
    var userId = UserContextExtension.GetUserId(User); 
    await _commandService.UpdateUserAsync(userId, request);
    
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      "User updated successfully", 
      DomainErrorCodes.AuthCodes.Success
    ));
  }
  
  [HasPermission(Permissions.Profile.Delete)]
  [HttpDelete("me")]
  public async Task<IActionResult> DeleteCurrentUser()
  {
    var userId = UserContextExtension.GetUserId(User);
    await _commandService.DeleteUserAsync(userId);
    
    return NoContent();
  }

  // [HasPermission(Permissions.Profile.ContractsRead)]
  [HttpGet("me/contracts")]
  public async Task<IActionResult> GetCurrentUserContracts()
  {
    var userId = UserContextExtension.GetUserId(User);
    var contracts = await _queryService.GetCurrentUserContractsAsync(userId, null);
    
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      "User contracts retrieved successfully", 
      DomainErrorCodes.AuthCodes.Success,
      contracts
    ));
  }




  //! [HasPermission(Permissions.Users.Update)]
  //! [HttpPut("{userId}")]
  //! public async Task<IActionResult> UpdateUserById([FromRoute] Guid userId, [FromBody] UpdateUserDto request)
  // {
  //   await _commandService.UpdateUserAsync(userId, request);

  //   return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
  //     HttpContext,
  //     HttpResponseState.Success,
  //     true,
  //     "User updated successfully",
  //     DomainErrorCodes.AuthCodes.Success
  //   ));
  // }

  //! [HasPermission(Permissions.Users.Delete)]
  //! [HttpDelete("{userId}")]
  //! public async Task<IActionResult> DeleteUserById([FromRoute] Guid userId)
  //! {
  //!   await _commandService.DeleteUserAsync(userId);
  //!   return NoContent();
  //! }
}