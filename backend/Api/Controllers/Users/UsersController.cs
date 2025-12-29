using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
  private readonly UserService _userService;
  private readonly TokenService _tokenService;
  private readonly AuthCookieService _authCookieService;
  private readonly ILogger _logger;

  public UsersController(
    UserService userService, 
    TokenService tokenService, 
    AuthCookieService authCookieService,
    ILogger<UsersController> logger
  )
  {
    _userService = userService;
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
    var users = await _userService.GetAllUsersAsync();
    
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
      "Users retrieved successfully", 
      HttpStatusCodes.AuthCodes.Success,
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
    var user = await _userService.GetUserByIdAsync(userId);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      true,
      "User retrieved successfully",
      HttpStatusCodes.AuthCodes.Success,
      user
    ));
  }

  [HasPermission(Permissions.Users.Update)]
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateUserById([FromRoute] Guid userId, [FromBody] UpdateUserDto request)
  {
    await _userService.UpdateUserByIdAsync(userId, request);

    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      true,
      "User updated successfully",
      HttpStatusCodes.AuthCodes.Success
    ));
  }

  [HasPermission(Permissions.Users.Delete)]
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteUserById([FromRoute] Guid userId)
  {
    await _userService.DeleteUserByIdAsync(userId);
    
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext,
      HttpResponseState.Success,
      true,
      "Deleted user successfully",
      HttpStatusCodes.AuthCodes.Success
    ));
  }

  // -------------------------------
  // path: /users/me         READ, UPDATE, DELETE
  // -------------------------------

  [HasPermission(Permissions.Profile.Read)]
  [HttpGet("me")]
  public async Task<IActionResult> GetCurrentUser()
  {
    var userId = GetUserClaims.GetUserId(User);
    var user = await _userService.GetCurrentUserAsync(userId);
    
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true, 
      "Current user retrieved successfully", 
      HttpStatusCodes.AuthCodes.Success,
      user
      ));
  }

  [HasPermission(Permissions.Profile.Update)]
  [HttpPut("me")]
  public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateCurrentUserDto request)
  {
    var userId = GetUserClaims.GetUserId(User);
    
    await _userService.UpdateCurrentUserAsync(userId, request);
    
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
      "User updated successfully", 
      HttpStatusCodes.AuthCodes.Success
    ));
  }
  
  [HasPermission(Permissions.Profile.Delete)]
  [HttpDelete("me")]
  public async Task<IActionResult> DeleteCurrentUser()
  {
    var userId = GetUserClaims.GetUserId(User);
    await _userService.DeleteCurrentUserAsync(userId);
    
    return Ok(HttpResponseFactory.CreateSuccessResponse<object>(
      HttpContext, 
      HttpResponseState.Success, 
      true,
      $"User with ID {userId} deleted successfully",
      HttpStatusCodes.AuthCodes.Success
    )); 
  }
}