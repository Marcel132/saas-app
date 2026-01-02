using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
  private readonly UserCommandService _commandService;
  private readonly UserQueryService _queryService;
  private readonly TokenService _tokenService;
  private readonly AuthCookieService _authCookieService;
  private readonly ILogger _logger;

  public UsersController(
    UserCommandService commandService, 
    UserQueryService queryService,
    TokenService tokenService, 
    AuthCookieService authCookieService,
    ILogger<UsersController> logger
  )
  {
    _commandService = commandService;
    _queryService = queryService;
    _tokenService = tokenService;
    _authCookieService = authCookieService;
    _logger = logger;
  }

  // -------------------------------
  // path: /users         READ
  // -------------------------------

  [HasPermission(Permissions.Users.Read)]
  [HttpGet]
  public async Task<IActionResult> GetUsers()
  {
    var users = await _queryService.GetAllAsync();
    
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
      "Users retrieved successfully", 
      DomainErrorCodes.AuthCodes.Success,
      users
      ));
  }

  // -------------------------------
  // path: /users/{id}         READ, UPDATE, DELETE
  // -------------------------------

  [HasPermission(Permissions.Users.Read)]
  [HttpGet("{id}")]
  public async Task<IActionResult> GetUserById([FromRoute] Guid userId)
  {
    var user = await _queryService.GetByIdAsync(userId);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      true,
      "User retrieved successfully",
      DomainErrorCodes.AuthCodes.Success,
      user
    ));
  }

  [HasPermission(Permissions.Users.Update)]
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateUserById([FromRoute] Guid userId, [FromBody] UpdateUserDto request)
  {
    await _commandService.UpdateUserAsync(userId, request);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      true,
      "User updated successfully",
      DomainErrorCodes.AuthCodes.Success
    ));
  }

  [HasPermission(Permissions.Users.Delete)]
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteUserById([FromRoute] Guid userId)
  {
    await _commandService.DeleteUserAsync(userId);
    
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      true,
      "Deleted user successfully",
      DomainErrorCodes.AuthCodes.Success
    ));
  }

  // -------------------------------
  // path: /users/me         READ, UPDATE, DELETE
  // -------------------------------

  [HasPermission(Permissions.Profile.Read)]
  [HttpGet("me")]
  public async Task<IActionResult> GetCurrentUser()
  {
    var userId = UserContextExtension.GetUserId(User);
    var user = await _queryService.GetByIdAsync(userId);
    
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true, 
      "Current user retrieved successfully", 
      DomainErrorCodes.AuthCodes.Success,
      user
      ));
  }

  [HasPermission(Permissions.Profile.Update)]
  [HttpPut("me")]
  public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserDto request)
  {
    var userId = UserContextExtension.GetUserId(User); 
    await _commandService.UpdateUserAsync(userId, request);
    
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
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
    
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
      $"User with ID {userId} deleted successfully",
      DomainErrorCodes.AuthCodes.Success
    )); 
  }
}