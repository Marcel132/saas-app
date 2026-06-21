public class ContractEvidence
{
    public long Id { get; private set; }

    public long VulnerabilityId { get; private set; }

    public string FilePath { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;
}