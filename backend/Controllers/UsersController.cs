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
  
  [Authorize(Roles = "Admin")]
  [HttpGet]
  public IActionResult GetUsers()
  {
    // This is a placeholder for the actual user retrieval logic.
    var users = _userService.GetAllUsersAsync().Result;
    return Ok(users);
  }


  [Authorize(Roles = "Admin")]
  [HttpGet("{id}")]
  public IActionResult GetUserById(int id)
  {
    if (id <= 0)
    {
      return BadRequest(new { Message = "Invalid user ID." });
    }
    // This is a placeholder for the actual user retrieval logic by ID.
    try
    {
      var user = _userService.GetUserByIdAsync(id).Result;
      return Ok(user);
    }
    catch (KeyNotFoundException ex)
    {
      return NotFound(new { Message = ex.Message });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { Message = "An error occurred while retrieving the user.", Details = ex.Message });
    }
  }

  [HttpPut("update/{id}")]
  public IActionResult UpdateUser(int id, [FromBody] object user)
  {
    // This is a placeholder for the actual user update logic.
    return Ok(new { Message = $"User with ID: {id} updated successfully", User = user });
  }

  [Authorize(Roles = "Admin")]
  [HttpDelete("delete/{id}")]
  public async Task<IActionResult> DeleteUser(int id)
  {
    if (id <= 0)
    {
      return BadRequest(new { Message = "Invalid user ID." });
    }

    try
    {
      return await _userService.DeleteUserAsync(id);
    }
    catch (ArgumentException ex)
    {
      return NotFound(new { Message = ex.Message });
    }
    catch (Exception)
    {
      throw new Exception("Nie udało się usunąć użytkownika {#003}");
    }
  }


  [HttpPost("account/login")]
  public IActionResult Login([FromBody] object loginDetails)
  {
    // This is a placeholder for the actual login logic.
    return Ok(new { Message = "Login successful", Details = loginDetails });
  }

  [HttpPost("account/logout")]
  public IActionResult Logout([FromBody] object logoutDetails)
  {
    // This is a placeholder for the actual logout logic.
    return Ok(new { Message = "Logout successful", Details = logoutDetails });
  }

  [HttpPost("account/new/register")]
  public async Task<IActionResult> Register([FromBody] RegisterRequestModel request)
  {
    // This is a placeholder for the actual registration logic.
    if (request.User == null || request.UserData == null)
    {
      return BadRequest(new { Message = "Brak wszystkich danych" });
    }

    try
    {
      var user = await _userService.RegisterUserInTableUsersAsync(request.User);
      if (user == null)
      {
        return BadRequest(new { Message = "Nie udało się zarejestrować użytkownika {#001}" });
      }
      request.UserData.UserId = user.Id;
      var userData = await _userService.RegisterUserDataInTableUserDataAsync(request.UserData);

      if (user == null || userData == null)
      {
        return BadRequest(new { Message = "Nie udało się zarejestrować użytkownika {#002}" });
      }
      else
      {
        return Ok(new { Message = "Użytkownik zarejestrowany pomyślnie", User = user, UserData = userData });
      }
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { Message = "An error occurred while registering the user. {#500}", Details = ex.Message });
    }

  }


  // Token

  [HttpPost("account/token/generate")]
  public async Task<IActionResult> GenerateToken([FromBody] TokenAuthModel request)
  {
    if (request.UserId <= 0 || string.IsNullOrEmpty(request.Role))
    {
      return BadRequest(new { Message = "UserId and Role are required." });
    }

    try
    {
      var token = await _tokenService.GenerateToken(request.UserId, request.Role);
      return Ok(new { Token = token });
    }
    catch (ArgumentException ex)
    {
      return BadRequest(new { Message = ex.Message });
    }
    catch (KeyNotFoundException ex)
    {
      return NotFound(new { Message = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
      return StatusCode(500, new { Message = "An error occurred while generating the token.", Details = ex.Message });
    }
  }
}