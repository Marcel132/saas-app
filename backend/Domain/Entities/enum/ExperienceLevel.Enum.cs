using System.Runtime.Serialization;

namespace backend.Domain.Entities.Enum;

public enum ExperienceLevel
{
  [EnumMember(Value = "None")]
  None,
  [EnumMember(Value = "Intern")]
  Intern,
  [EnumMember(Value = "Beginner")]
  Beginner,
  [EnumMember(Value = "Junior")]
  Junior,
  [EnumMember(Value = "Mid")]
  Mid,
  [EnumMember(Value = "Senior")]
  Senior
}