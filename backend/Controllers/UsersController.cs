using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    return Ok(new { success = true, data = users , message = "Users retrieved successfully."});
  }


  // path: /users/{id}
  // This endpoint retrieves a user by their ID.
  [Authorize(Roles = "Admin")]
  [HttpGet("{id}")]
  public async Task<IActionResult> GetUserById(int id)
  {
    // Validate the ID before proceeding.
    if (id <= 0)
    {
      return BadRequest(new { success = false, message = "Invalid user ID." });
    }

    // Attempt to retrieve the user by ID.
    // If the user is not found, return a 404 Not Found response.
    try
    {
      var user = await _userService.GetUserByIdAsync(id);
      return Ok( new { success = true, data = user, message = "User retrieved successfully." });
    }
    catch (KeyNotFoundException ex)
    {
      return NotFound(new { success = false, message = ex.Message });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { success = false, message = "An error occurred while retrieving the user.", details = ex.Message });
    }
  }


  // path: /users/{id}
  // This endpoint updates a user by their ID.
  [Authorize]
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserModel updateUser)
  {
    // Validate the ID and requested data before proceeding.
    // If the ID is invalid or requested data is null return a 400 Bad Request response. 
    if (id <= 0 || updateUser == null)
    {
      return BadRequest(new { success = false, message = "Invalid user ID or update data." });
    }

    if (!ModelState.IsValid)
    {
      return BadRequest( new { success = false, message = "Invalid model state.", details = ModelState });
    }

    // Attempt to update the user by ID.
      // If the user is not found, return a 404 Not Found response.
      try
      {
        var updatedUser = await _userService.UpdateUserAsync(id, updateUser);
        if (updatedUser != true)
        {
          return NotFound(new { success = false, message = $"User with ID {id} not found." });
        }

        return Ok(new { success = true, message = "User updated successfully." });
      }
      catch (ArgumentException ex)
      {
        return BadRequest(new { success = false, message = ex.Message });
      }
      catch (KeyNotFoundException ex)
      {
        return NotFound(new { success = false, message = ex.Message });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { success = false, message = "An error occurred whike updating the user.", details = ex.Message });
      }

  }


  // path: /users/{id}
  // This endpoint deletes a user by their ID.
  [Authorize(Roles = "Admin")]
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteUser(int id)
  {
    //  Validate the ID before proceeding.
    // If the ID is invalid, return a 400 Bad Request response.
    if (id <= 0)
    {
      return BadRequest(new { success = false, message = $"Invalid user ID: {id}." });
    }
    
    // Attempt to delete the user by ID.
    // If the user is not found, return a 404 Not Found response.
    try
    {
      await _userService.DeleteUserAsync(id);
      return Ok(new { success = true, message = $"User with ID {id} deleted successfully." });
    }
    catch (KeyNotFoundException ex)
    {
      return NotFound(new { success = false, message = ex.Message });
    }
    catch (ArgumentException ex)
    {
      return BadRequest(new { success = false, message = ex.Message });
    }
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
    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(request.Email);
    ArgumentNullException.ThrowIfNull(request.Password);

    if (!ModelState.IsValid)
    {
      return BadRequest(new { success = false, message = "Invalid login request." });
    }
    
    // Attempt to authenticate the user using the provided credentials.
    // If authentication fails, return a 401 Unauthorized response.
    try
    {
      var user = await _userService.AuthenticateUserAsync(request.Email, request.Password);
      if (user == null)
      {
        return Unauthorized(new { success = false, message = "Authentication failed. Invalid email or password." });
      }
      var token = await GenerateToken(new TokenAuthModel { UserId = user.Id, Role = user.Role.ToString() }) ?? throw new ArgumentException("Token generation failed.");
      return Ok(new { success = true, data = new { email = request.Email, authToken = token }, message = "Login successful." });
    }
    catch (ArgumentException ex)
    {
      return BadRequest(new { success = false, message = ex.Message });
    }
    catch (KeyNotFoundException ex)
    {
      return NotFound(new { success = false, message = ex.Message });
    }
    catch (UnauthorizedAccessException ex)
    {
      return Unauthorized(new { success = false, message = ex.Message });
    }
    catch (System.Exception ex)
    {
      return StatusCode(500, new { success = false, message = "An error occurred during authentication.", details = ex.Message });
    }
  }

  // path: /users/logout
  // This endpoint logs out a user by revoking their active sessions.
  [Authorize]
  [HttpPost("logout")]
  public async Task<IActionResult> Logout([FromBody] LogoutRequestModel request)
  {

    ArgumentNullException.ThrowIfNull(request);
    ArgumentNullException.ThrowIfNull(request.Email);

    if (!ModelState.IsValid)
    {
      return BadRequest(new { success = false, message = "Invalid logout request." });
    }

    var deviceIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

    try
    {
      var user = await _userService.LogoutUserAsync(request.Email, deviceIp);
      if (user == false)
      {
        return BadRequest(new { success = false, message = "Cannot delete a session" });
      }

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
    catch (KeyNotFoundException ex)
    {
      return NotFound(new { success = false, message = ex.Message });
    }
    catch (ArgumentException ex)
    {
      return BadRequest(new { success = false, message = ex.Message });
    }
    catch (System.Exception)
    {
      throw;
    }
  }


  // Registration Endpoint
  // path: /users/register
  [AllowAnonymous]
  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterRequestModel request)
  {
    // Validate the request model.
    // If the request is null, return a 400 Bad Request response.
    if (request == null)
    {
      return BadRequest(new { success = false, message = "Invalid registration request." });
    }
    if(!ModelState.IsValid)
    {
      return BadRequest(new { success = false, message = "Invalid model state.", details = ModelState });
    }

    // Attempt to register the user using the provided model.
    // If the user registration fails, return a 400 Bad Request response.
    try
    {
      var user = await _userService.RegisterModelAsync(request);
      if (user == null)
      {
        return BadRequest(new { success = false, message = "User registration failed. User already exists or invalid data." });
      }
      if (request.User == null)
      {
        return BadRequest(new { success = false, message = "User information is missing in the registration request." });
      }

      var token = await GenerateToken(new TokenAuthModel { UserId = user.Id, Role = request.User.Role.ToString()}) ?? throw new ArgumentException("Token generation failed.");

      return Ok(new { success = true, data = new { email = user.Email, id = user.Id, authToken = token }, message = "User registered successfully." });
    }
    catch (ArgumentException ex)
    {
      return BadRequest(new { success = false, message = ex.Message });
    }

    catch (System.Exception)
    {
      return StatusCode(500, new { success = false, message = "An error occurred while registering the user." });
    }
  }


  // Token

  // Generation Endpoint 
  // path: /users/token/generate
  [HttpPost("token/generate")]
  public async Task<IActionResult> GenerateToken([FromBody] TokenAuthModel request)
  {
    if (request.UserId <= 0 || string.IsNullOrEmpty(request.Role))
    {
      return BadRequest(new { success = false,  message = "UserId and Role are required." });
    }
    if (!ModelState.IsValid)
    {
      return BadRequest(new { success = false, message = "Invalid model state.", details = ModelState });
    }

    try
    {
      var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
      var userAgent = HttpContext.Request.Headers["User-Agent"].ToString() ?? "Unknown User Agent";

      var token = await _tokenService.GenerateAuthToken(request.UserId, request.Role, ip, userAgent);

      // Set the refresh token as an HttpOnly cookie
      Response.Cookies.Append("RefreshToken", token.RefreshToken, new CookieOptions
      {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddDays(7)
      });

      return Ok(new { success = true, data = new { authToken = token.AuthToken }, message = "Token generated successfully." });
    }
    catch (ArgumentException ex)
    {
      return BadRequest(new { success = false, message = ex.Message });
    }
    catch (KeyNotFoundException ex)
    {
      return NotFound(new { success = false, message = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
      return StatusCode(500, new { success = false, message = "An error occurred while generating the token.", details = ex.Message });
    }
  }
}