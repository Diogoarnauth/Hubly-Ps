public class ConversationOutputModel
{
    public int Id { get; set; }
    public string LastMessage { get; set; } = string.Empty;
    public long LastMessageAt { get; set; }
    public string OtherPartyName { get; set; } = string.Empty;
    public int? PlatformId { get; set; }
    public int UnreadCount { get; set; }
    public ConversationTagOutputModel? Tag { get; set; }
}