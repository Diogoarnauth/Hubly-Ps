using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hubly.api.Domain.Entities;

[Table("co_workers", Schema = "dbo")]
public class CoWorker
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("owner_id")]
    public int OwnerId { get; set; }

    [Column("joined_at")]
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
   
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("OwnerId")]
    public virtual User Owner { get; set; } = null!;
}