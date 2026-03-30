namespace Hubly.api.DTOs;

public class ProfileHistoryOutputModel
{
    public int Id { get; set; }
    public DateTime ViewedAt { get; set; }
    
    public string TargetName { get; set; } = null!;
    public string TargetType { get; set; } = null!;
    public int TargetId { get; set; }
}