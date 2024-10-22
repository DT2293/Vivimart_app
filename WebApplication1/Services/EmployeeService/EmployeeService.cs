using Microsoft.EntityFrameworkCore;
using WebApplication1.Helper;
using WebApplication1.Models;
using WebApplication1.ViewModels.Employee;

namespace WebApplication1.Services
{
    public class EmployeeService
    {
        private readonly AuthContext _db;
        private readonly CustomPasswordHasher _customPasswordHasher;
        public EmployeeService(AuthContext db, CustomPasswordHasher customPasswordHasher)
        {
            _db = db;
            _customPasswordHasher = customPasswordHasher;
        }

        public bool UpdateInfo(EmployeePageViewModel model, out string errorMessage)
        {
            errorMessage = string.Empty;
            var user = _db.Users.FirstOrDefault(u => u.Username == model.newUsername);
            if (user != null)
            {
                errorMessage = "Tên người dùng không tồn tại.";
                return false;
            }

            if (model.Password != model.ComfirmPassword)
            {
                errorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.";
                return false;
            }

            var hashedPassword = _customPasswordHasher.HashPassword(model.Password);
            user.Password = hashedPassword;
            _db.Users.Update(user);
            _db.SaveChanges();

            return true;

        }

      
    }
}
