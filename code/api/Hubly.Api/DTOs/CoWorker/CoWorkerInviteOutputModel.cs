using System;
using System.ComponentModel.DataAnnotations;

namespace Hubly.api.DTOs;

public class CoWorkerInviteOutputModel   
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    [Required]
    [EmailAddress]
    public string CoWorkerEmail { get; set; } = null!;

    [Required]
    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }
}