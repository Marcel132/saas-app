public class UserSummaryDto
{
  public int ActiveTasks { get; set; }
  public int ActiveOrders { get; set; }
  public int CompletedReports { get; set; }
  public int TotalReports { get; set; }
  public DateTime LastActivity { get; set; }
}