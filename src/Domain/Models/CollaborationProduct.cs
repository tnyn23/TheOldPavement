using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class CollaborationProduct
{
    public int Id { get; set; }

    public int CollaborationId { get; set; }

    public int ProductId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Collaboration Collaboration { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}


