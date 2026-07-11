namespace Hubly.api.DTOs;

public class GetMyCoWorkerWithEmailOutputModel 
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int OwnerId { get; set; }
    public DateTime JoinedAt { get; set; }
    public string CoWorkerEmail { get; set; } = string.Empty;
}