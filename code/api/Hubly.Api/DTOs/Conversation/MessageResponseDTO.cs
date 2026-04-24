namespace Hubly.api.DTOs;

    public class MessageResponseDTO
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public long SentAt { get; set; }
        public bool IsEdited { get; set; }
    }
