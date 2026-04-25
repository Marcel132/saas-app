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
public enum ContractExecutionStatus
{
  [EnumMember(Value = "completed")]
  Completed,
  [EnumMember(Value = "in_progress")]
  InProgress,
  [EnumMember(Value = "not_started")]
  NotStarted
}
