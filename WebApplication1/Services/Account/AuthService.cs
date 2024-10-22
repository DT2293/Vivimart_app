using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Services.Account
{
    public class AuthService
    {
        private readonly AuthContext _db;
        private readonly CustomPasswordHasher _customPasswordHasher;
        public AuthService(AuthContext db, CustomPasswordHasher customPasswordHasher)
        {
            _db = db;
            _customPasswordHasher = customPasswordHasher;
        }
        public async Task<InfoViewModel> LoginAsync(string username, string password)
        {
            InfoViewModel infoViewModel = new InfoViewModel();

            var user = await _db.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return null;
            }

            var isPasswordValid = _customPasswordHasher.VerifyPassword(user.Password, password);

            if (!isPasswordValid)
            {
                return null;
            }

           
            var userRole = user.UserRoles.FirstOrDefault().Role.RoleName;

            return new InfoViewModel
            {
                Username = user.Username,
                RoleName = userRole
            };
        }
    }
}
