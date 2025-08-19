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

        return Ok(new { success = true, data = updatedUser, message = "User updated successfully." });
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
  [AllowAnonymous]
  [HttpPost("login")]
  public IActionResult Login([FromBody] LoginRequestModel request)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(new { success = false, message = "Invalid login request." });
    }
    // This is a placeholder for the actual login logic.
    return Ok(new { success = true, data = request, message = "Login successful." });
  }

  // path: /users/logout
  [HttpPost("logout")]
  public IActionResult Logout([FromBody] LogoutRequestModel request)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(new { success = false, message = "Invalid logout request." });
    }

    // This is a placeholder for the actual logout logic.
      return Ok(new { success = true, data = request, message = "Logout successful." });
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

      return Ok(new { success = true, data = new { email = user.Email, id = user.Id }, message = "User registered successfully." });
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

      var token = await _tokenService.GenerateToken(request.UserId, request.Role, ip, userAgent);
      return Ok(new { success = true, data = new { token }, message = "Token generated successfully." });
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