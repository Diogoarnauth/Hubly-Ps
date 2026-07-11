namespace Hubly.api.DTOs;

public class GetMyCoWorkerOutputModel 
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int OwnerId { get; set; }
    public DateTime JoinedAt { get; set; }
}