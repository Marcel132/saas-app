using System.Runtime.Serialization;

public enum ContractStatus
{
  [EnumMember(Value = "open")]
  Open,
  [EnumMember(Value = "in_progress")]
  InProgress,
  [EnumMember(Value = "completed")]
  Completed,
  [EnumMember(Value = "cancelled")]
  Cancelled
}
public enum ContractApplicationStatus
{
  [EnumMember(Value = "accepted")]
  Accepted,
  [EnumMember(Value = "pending")]
  Pending,
  [EnumMember(Value = "rejected")]
  Rejected
}
public enum ContractReportStatus
{
  [EnumMember(Value = "draft")]
  Draft,
  [EnumMember(Value = "submitted")]
  Submitted,
  [EnumMember(Value = "changes_requested")]
  ChangesRequested,
  [EnumMember(Value = "approved")]
  Approved,
  [EnumMember(Value = "rejected")]
  Rejected
}
