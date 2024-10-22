using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class CreateUserViewModel
    {
        public User User { get; set; } = new User(); // Khởi tạo User
        public string SelectedRole { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> Error { get; set; } = new List<string>();
    }
}
