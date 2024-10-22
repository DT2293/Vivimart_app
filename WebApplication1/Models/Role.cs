using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
