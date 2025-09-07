using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ContractRequestModel
{
  public decimal Price { get; set; }
  public StatusOfContractModel Status { get; set; }
  public string? Description { get; set; }
  public DateTime? Deadline { get; set; }

  [ForeignKey("AuthorId")]
  public UsersModel? Author { get; set; }
  [ForeignKey("TargetId")]
  public UsersModel? Target { get; set; }
}