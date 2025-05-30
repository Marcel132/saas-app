using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class UserService
{
  private readonly AppDbContext _context;
  public UserService(AppDbContext context)
  {
    _context = context;
  }

  public async Task<List<UsersModel>> GetAllUsersAsync()
  {
    var users = await _context.users
    .Include(u => u.Session)
    .Include(u => u.UserData)
    .Include(u => u.Opinions)
    .Include(u => u.ApiLogs)
    .ToListAsync();

    return users;
  }

  public async Task<UsersModel> GetUserByIdAsync(int userId)
  {
    var user = await _context.users
      .Include(u => u.Session)
      .Include(u => u.UserData)
      .Include(u => u.Opinions)
      .Include(u => u.ApiLogs)
      .FirstOrDefaultAsync(u => u.Id == userId);


    if (user == null)
    {
      throw new KeyNotFoundException($"User with ID {userId} not found.");
    }
    else
    {
      return user;
    }
  }

  public async Task<UsersModel> RegisterUserInTableUsersAsync(UsersModel user)
  {
    if (user == null)
    {
      throw new ArgumentNullException("User data is null.");
    }
    else if (user.Email == null || user.PasswordHash == null)
    {
      throw new ArgumentNullException("Email and PasswordHash are required.");
    }

    try
    {
      // Check if the user already exists
      var existingUser = await _context.users.FirstOrDefaultAsync(u => u.Email == user.Email);
      if (existingUser != null)
      {
        throw new ArgumentException("User with this email already exists.");
      }

      // Add the new user to the database
      _context.users.Add(user);
      await _context.SaveChangesAsync();

      return user;
    }
    catch (Exception ex)
    {
      throw new Exception(new { Message = "An error occurred while registering the user.", Details = ex.Message }.ToString());
    }
  }
  
  public async Task<IActionResult> RegisterUserDataInTableUserDataAsync(UserDataModel userData)
  {
    if (userData == null)
    {
      return new BadRequestObjectResult("User data is null.");
    }
    else if (userData.UserId <= 0)
    {
      return new BadRequestObjectResult("UserId is required.");
    }
    else if (string.IsNullOrEmpty(userData.FirstName) || string.IsNullOrEmpty(userData.LastName))
    {
      return new BadRequestObjectResult("FirstName and LastName are required.");
    }
    else if (string.IsNullOrEmpty(userData.PhoneNumber) || string.IsNullOrEmpty(userData.Country) ||
             string.IsNullOrEmpty(userData.PostalCode) || string.IsNullOrEmpty(userData.Street))
    {
      return new BadRequestObjectResult("PhoneNumber, Country, PostalCode, and Street are required.");
    }

    try
    {
      // Check if the user data already exists
      var existingUserData = await _context.user_data.FirstOrDefaultAsync(ud => ud.UserId == userData.UserId);
      if (existingUserData != null)
      {
        return new ConflictObjectResult("User data for this user already exists.");
      }

      // Add the new user data to the database
      _context.user_data.Add(userData);
      await _context.SaveChangesAsync();

      return new OkObjectResult(userData);
    }
    catch (Exception ex)
    {
      return new ObjectResult(new { Message = "An error occurred while registering the user.", Details = ex.Message }) { StatusCode = 500 };
    }
  }
}
