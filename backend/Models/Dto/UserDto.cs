public class UserDto
{
  public int Id { get; set; }
  public string Email { get; set; } = string.Empty;
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public List<string> Specialization { get; set; } = new List<string>();
  public string Skills { get; set; } = string.Empty;
}