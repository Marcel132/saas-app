using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
  private readonly UserService _userService;
  private readonly TokenService _tokenService;

  public UsersController(UserService userService, TokenService tokenService)
  {
    _userService = userService;
    _tokenService = tokenService;
  }


  // User Management Endpoints
  // path: /users
  [Authorize(Roles = "Admin")]
  [HttpGet]
  public async Task<IActionResult> GetUsers()
  {
    // This endpoint retrieves all users.
    // It includes their session, user data, opinions, and API logs.
    var users = await _userService.GetAllUsersAsync();
    return Ok(new { success = true, data = users, message = "Users retrieved successfully." });
  }

  [Authorize]
  [HttpGet("me")]
  public async Task<IActionResult> GetCurrentUser()
  {
    if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
      return Unauthorized(new { success = false, message = "Invalid or missing user ID claim." });

    try
    {
      var user = await _userService.GetCurrentUserAsync(userId);

      return Ok(new { success = true, data = user, message = "Token retrieved successfully." });
    }
    catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
    catch (KeyNotFoundException ex) { return NotFound(new { success = false, message = ex.Message }); }
    catch (System.Exception) { return StatusCode(500, new { success = false, message = "An error occurred while getting current user" }); }
  }

  // path: /users/{id}
  // This endpoint updates a user by their ID.
  [Authorize]
  [HttpPut]
  public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserModel request)
  {
    // Validate the ID and requested data before proceeding.
    // If the ID is invalid or requested data is null return a 400 Bad Request response. 
    // ArgumentNullException.ThrowIfNull(request, "Request cannot be null");
    if (!ModelState.IsValid)
      return BadRequest(new { success = false, message = "Invalid model state", details = ModelState });

    if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
      return Unauthorized(new { success = false, message = "Invalid or missing user ID claim." });

    // Attempt to update the user by ID.
    // If the user is not found, return a 404 Not Found response.
    try
    {

      var user = await _userService.UpdateUserAsync(userId, request);
      return Ok(new { success = true, message = "User updated succesfulty" });
    }
    catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
    catch (KeyNotFoundException ex) { return NotFound(new { success = false, message = ex.Message }); }
    catch (System.Exception)
    {
      return StatusCode(500, new { success = false, message = "An error occurred while updating a user" });
    }
  }


  // path: /users/{id}
  // This endpoint deletes a user by their ID.
  [Authorize]
  [HttpDelete]
  public async Task<IActionResult> DeleteUser()
  {
    //  Validate the ID before proceeding.
    // If the ID is invalid, return a 400 Bad Request response.
    if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
      return Unauthorized(new { success = false, message = "Invalid or missing user ID claim." });

    // Attempt to delete the user by ID.
    // If the user is not found, return a 404 Not Found response.
    try
    {
      await _userService.DeleteUserAsync(userId);
      return Ok(new { success = true, message = $"User with ID {userId} deleted successfully." });
    }
    catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
    catch (KeyNotFoundException ex) { return NotFound(new { success = false, message = ex.Message }); }
    catch (System.Exception)
    {
      return StatusCode(500, new { success = false, message = "An error occurred while deleting the user." });
    }
  }

  // path: /users/login
  // This endpoint authenticates a user and generates an auth token.
  [AllowAnonymous]
  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
  {

    // Validate the request model.
    // ArgumentNullException.ThrowIfNull(request, "Request cannot be null");
    ArgumentNullException.ThrowIfNull(request.Email, "Email cannot be null");
    ArgumentNullException.ThrowIfNull(request.Password, "Password cannot be null");

    if (!ModelState.IsValid)
      return BadRequest(new { success = false, message = "Invalid login request." });

    // Attempt to authenticate the user using the provided credentials.
    // If authentication fails, return a 401 Unauthorized response.
    try
    {
      var user = await _userService.AuthenticateUserAsync(request)
        ?? throw new UnauthorizedAccessException("Authentication failed! Invalid email or password");

      var token = await GenerateToken(new TokenAuthModel { UserId = user.Id, Role = user.Role.ToString() })
        ?? throw new ArgumentException("Token generation failed.");

      return Ok(new { success = true, message = "Login successful." });
    }
    catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
    catch (KeyNotFoundException ex) { return NotFound(new { success = false, message = ex.Message }); }
    catch (UnauthorizedAccessException ex) { return Unauthorized(new { success = false, message = ex.Message }); }
    catch (System.Exception ex)
    {
      return StatusCode(500, new { success = false, message = "An error occurred during authentication.", details = ex.Message });
    }
  }

  // path: /users/logout
  // This endpoint logs out a user by revoking their active sessions.
  [Authorize]
  [HttpPost("logout")]
  public async Task<IActionResult> Logout()
  {

    if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
      return Unauthorized(new { success = false, message = "Invalid or missing user ID claim." });

    var deviceIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

    try
    {
      var user = await _userService.LogoutUserAsync(userId, deviceIp);
      if (!user)
        return BadRequest(new { success = false, message = "Cannot delete a session" });

      // Clear the auth token cookie
      Response.Cookies.Append("AuthToken", "", new CookieOptions
      {
        Expires = DateTime.UtcNow.AddDays(-1),
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict
      });
      // Clear the refresh token cookie
      Response.Cookies.Append("RefreshToken", "", new CookieOptions
      {
        Expires = DateTime.UtcNow.AddDays(-1),
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict
      });

      return Ok(new { success = true, message = "Logout successful." });
    }
    catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
    catch (KeyNotFoundException ex) { return NotFound(new { success = false, message = ex.Message }); }
    catch (System.Exception)
    {
      return StatusCode(500, new { success = false, message = "An error occurred while logouta user" });
    }
  }

  // Registration Endpoint
  // path: /users/register
  [AllowAnonymous]
  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] UsersModel request)
  {
    // Validate the request model.
    // If the request is null, return a 400 Bad Request response.
    // ArgumentNullException.ThrowIfNull(request, "Request cannot be null");

    if (!ModelState.IsValid)
      return BadRequest(new { success = false, message = "Invalid model state.", details = ModelState });

    // Attempt to register the user using the provided model.
    // If the user registration fails, return a 400 Bad Request response.
    try
    {
      var user = await _userService.RegisterUserAsync(request);
      ArgumentNullException.ThrowIfNull(user, "User cannot be null");
      ArgumentNullException.ThrowIfNull(user.UserData, "UserData cannot be null");

      var token = await GenerateToken(new TokenAuthModel { UserId = user.Id, Role = user.Role.ToString() }) ?? throw new ArgumentException("Token generation failed.");

      return Ok(new { success = true, data = new { email = user.Email, id = user.Id, authToken = token }, message = "User registered successfully." });
    }
    catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
    catch (System.Exception)
    {
      return StatusCode(500, new { success = false, message = "An error occurred while registering the user." });
    }
  }

  // Generation Endpoint 
  // path: /users/token/generate
  [Authorize]
  [HttpPost("token/auth")]
  public async Task<IActionResult> GenerateToken([FromBody] TokenAuthModel request)
  {
    if (request.UserId <= 0 || string.IsNullOrEmpty(request.Role))
      return BadRequest(new { success = false, message = "UserId and Role are required." });

    if (!ModelState.IsValid)
      return BadRequest(new { success = false, message = "Invalid model state.", details = ModelState });

    try
    {
      var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
      var userAgent = HttpContext.Request.Headers["User-Agent"].ToString() ?? "Unknown User Agent";

      var token = await _tokenService.GenerateAuthToken(request.UserId, request.Role, ip, userAgent)
        ?? throw new ArgumentException("Token generation failed.");

      Response.Cookies.Append("AuthToken", token.AuthToken, new CookieOptions
      {
        HttpOnly = true,
        Secure = false,
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddMinutes(15)
      });

      // Set the refresh token as an HttpOnly cookie
      Response.Cookies.Append("RefreshToken", token.RefreshToken, new CookieOptions
      {
        HttpOnly = true,
        Secure = false,
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddDays(7)
      });

      return Ok(new { success = true, data = new { authToken = token.AuthToken }, message = "Token generated successfully." });
    }
    catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
    catch (KeyNotFoundException ex) { return NotFound(new { success = false, message = ex.Message }); }
    catch (InvalidOperationException ex)
    {
      return StatusCode(500, new { success = false, message = "An error occurred while generating the token.", details = ex.Message });
    }
  }

  [HttpPost("token/auth/refresh-token")]
  public async Task<IActionResult> RefreshToken()
  {
    if (!Request.Cookies.TryGetValue("RefreshToken", out string? refreshToken))
      return Unauthorized(new { success = false, message = "Refresh token is missing" });

    try
    {
      var token = await _tokenService.RefreshTokenAsync(refreshToken);

      if (token == null)
        return BadRequest("Cannot refresh a token");

      Response.Cookies.Append("AuthToken", token.AuthToken, new CookieOptions
      {
        HttpOnly = true,
        Secure = false,
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddMinutes(15)
      });

      // Set the refresh token as an HttpOnly cookie
      Response.Cookies.Append("RefreshToken", token.RefreshToken, new CookieOptions
      {
        HttpOnly = true,
        Secure = false,
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddDays(7)
      });

      return Ok(new { success = true, message = "Token refresh successfuly" });
    }
    catch (ArgumentException ex) { return BadRequest(new { success = false, message = ex.Message }); }
    catch (UnauthorizedAccessException ex) { return Unauthorized(new { success = false, message = ex.Message }); }
    catch (System.Exception)
    {

      throw;
    }
  }
}