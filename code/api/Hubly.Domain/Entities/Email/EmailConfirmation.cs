using System;
using System.Collections.Generic;

namespace Hubly.api.Domain.Entities;

public partial class EmailConfirmation
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string ConfirmationCode { get; set; } = null!;

    public long CreatedAt { get; set; }

    public long ExpiresAt { get; set; }

    public bool Used { get; set; }

    public virtual User User { get; set; } = null!;
}