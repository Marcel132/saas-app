public class ContractDto
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public int TargetId { get; set; }
    public decimal Price { get; set; }
    public StatusOfContractModel Status { get; set; }
    public string Description { get; set; } = "";
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? Deadline { get; set; }
    public List<ContractApplicationDto> Applications { get; set; } = new();
}