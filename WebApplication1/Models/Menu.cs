using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Menu
{
    public int MenuId { get; set; }

    public string MenuName { get; set; } = null!;

    public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
}
