namespace Hubly.api.DTOs;

public class AuditLogOutputModel
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public int? CoWorkerId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string Payload { get; set; } = "{}"; 
}