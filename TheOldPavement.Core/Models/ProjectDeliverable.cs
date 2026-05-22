using System;
using System.Collections.Generic;

namespace TheOldPavement.Core.Models;

public partial class ProjectDeliverable
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? IconName { get; set; }

    public string? Specifications { get; set; }

    public string? Link { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual CommercialProject Project { get; set; } = null!;
}
