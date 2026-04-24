using Hubly.api.Domain.Entities;

namespace Hubly.api.Domain.Entities;

public class ConversationParticipant
{
    public int ConversationId { get; set; }
    public int UserId { get; set; }
    
    // Opcionais: Definem se o user está a agir como empresa ou criador nesta conversa
    public int? CompanyId { get; set; }
    public int? SocialProfileId { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual Company? Company { get; set; }
    public virtual CreatorSocialProfile? SocialProfile { get; set; }
}