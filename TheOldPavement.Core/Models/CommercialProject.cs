using System;
using System.Collections.Generic;

namespace TheOldPavement.Core.Models;

public partial class CommercialProject
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ProjectDeliverable> ProjectDeliverables { get; set; } = new List<ProjectDeliverable>();

    public virtual ICollection<ProjectProduct> ProjectProducts { get; set; } = new List<ProjectProduct>();
}
