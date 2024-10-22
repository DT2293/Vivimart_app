using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class RoleMenu
{
    public int RoleId { get; set; }

    public int MenuId { get; set; }

    public string? AccessType { get; set; }

    public virtual Menu Menu { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
