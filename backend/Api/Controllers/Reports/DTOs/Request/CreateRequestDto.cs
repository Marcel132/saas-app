namespace backend.Api.Controllers.Reports.DTOs;

// TODO: Add FluentValidation
public class CreateRequestDto
{
  public string Title { get; set; } = string.Empty;
  public string Url { get; set; } = string.Empty;
  public string Scope { get; set; } = string.Empty;
  public string Credentials { get; set; } = string.Empty;
  public DateOnly Deadline { get; set; }
}