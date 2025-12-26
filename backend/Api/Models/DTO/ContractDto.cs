public class ContractDto
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public Guid TargetId { get; set; }
    public decimal Price { get; set; }
    public StatusOfContractModel Status { get; set; }
    public string Description { get; set; } = "";
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? Deadline { get; set; }
    public List<ContractApplicationDto> Applications { get; set; } = new();
}