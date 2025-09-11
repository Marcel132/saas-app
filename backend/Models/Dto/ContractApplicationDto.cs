public class ContractApplicationDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; }
}

public class ContractAcceptUserDto
{
    public int UserId { get; set; }

    public AcceptEnum Accpeted { get; set; }
    
}