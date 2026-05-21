using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;
 public class CreateConversationInputModel
{
    [Required(ErrorMessage = "Sender information is required.")]
    public ParticipantInputModel Sender { get; set; } = null!;
    [Required(ErrorMessage = "Receiver information is required.")]
    public ParticipantInputModel Receiver { get; set; } = null!;
}

public class ParticipantInputModel
{
    [Required(ErrorMessage = "Profile ID is required.")]
    public int ProfileId { get; set; } 
    [Required(ErrorMessage = "Participant type is required.")]
    public ParticipantType Type { get; set; } 
}

public enum ParticipantType { Company = 1, SocialProfile = 2 }