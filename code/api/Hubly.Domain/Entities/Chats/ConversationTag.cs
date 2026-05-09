namespace Hubly.api.Domain.Entities;

public class ConversationTag
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string TagName { get; set; } = string.Empty;
    public string ColorHex { get; set; } = "#808080";
    public long CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
