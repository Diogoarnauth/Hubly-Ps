using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hubly.api.Domain.Entities;

[Table("tokens", Schema = "dbo")]
public class Token
{
    
    [Key]
    [Column("token_validation")]
    public string TokenValidation { get; set; } = string.Empty;

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("created_at")]
    public long CreatedAt { get; set; } 

    [Column("last_used_at")]
    public long LastUsedAt { get; set; }

    public Token() { }

    public Token(string tokenValidation, int userId, long createdAt, long lastUsedAt)
    {
        TokenValidation = tokenValidation;
        UserId = userId;
        CreatedAt = createdAt;
        LastUsedAt = lastUsedAt;
    }
}