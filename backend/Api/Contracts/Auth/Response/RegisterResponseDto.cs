public class RegisterResponseDto
{
  public Guid Id { get; set; }
  public required HashSet<string> Permissions { get; set; }
}