public class ConversationSummaryDto
{
    public int Id { get; set; }
    public string LastMessage { get; set; } = string.Empty;
    public DateTime? LastMessageAt { get; set; }
    public string OtherPartyName { get; set; } = string.Empty;
    public int? PlatformId { get; set; } 
}