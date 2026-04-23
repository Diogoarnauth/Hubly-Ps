using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;
 public class CreateConversationInputModel
{
    public ParticipantInputModel Sender { get; set; } = null!;

    public ParticipantInputModel Receiver { get; set; } = null!;
}

public class ParticipantInputModel
{
    public int ProfileId { get; set; } 
    public ParticipantType Type { get; set; } 
}

public enum ParticipantType { Company = 1, SocialProfile = 2 }