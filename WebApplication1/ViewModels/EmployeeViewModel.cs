using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class EmployeeViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public string AccessType { get; set; }
       
        public List<User> Users { get; set; } 
        public List<string> Roles { get; set; }

    }
    public class EmpoyeeViewModel
    {
         public string keyWord { get; set; }
         public List<EmployeeViewModel> employeeViewModels { get; set; }
    }
}
