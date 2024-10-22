using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class UpdateUserRoleViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        public string Password { get; set; }
        public string NewEmail { get; set; }
        public string NewPassword { get; set; }
        public List<String> CurrentRole { get; set; }
        public string NewRoleId { get; set; }
        public List<Role> AvailableRoles { get; set; }

    }
}
