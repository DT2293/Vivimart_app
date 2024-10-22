using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models;

public partial class User
{
    [Key]
    public int UserId { get; set; } // Khóa chính

    public string Username { get; set; } = null!; // Tên đăng nhập của người dùng

    // Thuộc tính lưu mật khẩu đã hash
    //public string PasswordHash { get; set; } = null!; 

    // Thuộc tính không ánh xạ vào cơ sở dữ liệu, chỉ dùng để nhập mật khẩu từ người dùng
    //[NotMapped]
    public string Password { get; set; } = null!; // Mật khẩu người dùng nhập

    public string Email { get; set; } = null!; // Địa chỉ email của người dùng

    // Quan hệ giữa User và UserRole (nhiều-nhiều)
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    // Quan hệ giữa User và Invoice (một-nhiều)
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
