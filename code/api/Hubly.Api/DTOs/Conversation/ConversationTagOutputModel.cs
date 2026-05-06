using Hubly.api.Domain.Entities;

namespace Hubly.api.DTOs;

public class ConversationTagOutputModel
{
    public int Id { get; set; }
    public string TagName { get; set; } = string.Empty;
    public string ColorHex { get; set; } = "#808080";
    public long CreatedAt { get; set; }

    public static ConversationTagOutputModel FromEntity(ConversationTag tag)
    {
        return new ConversationTagOutputModel
        {
            Id = tag.Id,
            TagName = tag.TagName,
            ColorHex = tag.ColorHex,
            CreatedAt = tag.CreatedAt
        };
    }
}
