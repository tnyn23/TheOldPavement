using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class NewsletterSubscriber
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public DateTime? SubscribedAt { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? UnsubscribedAt { get; set; }

    public DateTime? CreatedAt { get; set; }
}


