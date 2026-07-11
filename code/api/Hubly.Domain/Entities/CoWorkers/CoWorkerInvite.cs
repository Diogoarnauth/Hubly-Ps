using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hubly.api.Domain.Entities;

[Table("co_worker_invites", Schema = "dbo")]
public class CoWorkerInvite
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("owner_id")]
    public int OwnerId { get; set; }

    [Column("co_worker_email")]
    [Required]
    [StringLength(150)]
    public string CoWorkerEmail { get; set; } = null!;

    [Column("status")]
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "WAITING"; 

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }

    [ForeignKey("OwnerId")]
    public virtual User Owner { get; set; } = null!;
}