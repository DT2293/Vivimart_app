    using System.ComponentModel.DataAnnotations;

    namespace WebApplication1.ViewModels
    {
        public class UpdateAccountViewModel

        {
            public int UserId { get; set; }
            [Required(ErrorMessage = "Tên người dùng là bắt buộc.")]
            public string Username { get; set; }
            [Required(ErrorMessage = "Tên người dùng là bắt buộc.")]
            public string NewUsername { get; set; }

            [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
            [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc.")]
            [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
            public string ConfirmPassword { get; set; }
        }
    }
