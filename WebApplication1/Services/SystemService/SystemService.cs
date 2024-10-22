using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;
using WebApplication1.ViewModels;

namespace WebApplication1.Services.SystemService
{
    public class SystemService
    {
        private readonly AuthContext _db;
        private readonly CustomPasswordHasher _customPasswordHasher;


        public SystemService(AuthContext db, CustomPasswordHasher customPasswordHasher)
        {
            _db = db;
            _customPasswordHasher = customPasswordHasher;
        }

        public int AddUserWithRoleAsync(string username, string password, string email, string roleName)
        {
            var kt = _db.Users.Where(x => x.Username == username).FirstOrDefault();
            if (kt == null)
            {
                if (password.Length >= 8)
                {
                    try
                    {
                        var haspas = _customPasswordHasher.HashPassword(password);
                        var parameters = new[]
                        {
                new SqlParameter("@Username", SqlDbType.VarChar) { Value = username },
                new SqlParameter("@Password", SqlDbType.VarChar) { Value = haspas },
                new SqlParameter("@Email", SqlDbType.VarChar) { Value = email },
                new SqlParameter("@RoleName", SqlDbType.VarChar) { Value = roleName },
               // new SqlParameter("@PasswordHash", SqlDbType.VarChar) { Value = haspas }
            };

                        _db.Database.ExecuteSqlRaw("EXEC AddUserWithRole @Username, @Password, @Email, @RoleName", parameters);
                        return 0; // ddung

                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }
                return 1; // sai mk

            }
            else
            {
                return 2; // datontai
            }

        }
   
        public bool UpdatePassword(UpdateAccountViewModel model, out string errorMessage)
        {
            errorMessage = string.Empty;
            var user = _db.Users.FirstOrDefault(u => u.Username == model.Username);
            if (user == null)
            {
                errorMessage = "Tên người dùng không tồn tại.";
                return false;
            }

            if (model.Password != model.ConfirmPassword)
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
        //public int UpdateUserAccount(int userId, string newUsername, string newPassword)
        //{
        //    // Tìm người dùng theo userId
        //    var user = _db.Users.FirstOrDefault(u => u.UserId == userId);

        //    // Kiểm tra nếu người dùng không tồn tại
        //    if (user == null)
        //    {
        //        return -1; // Người dùng không tồn tại
        //    }

        //    // Kiểm tra username có tồn tại không, nhưng không tính đến user hiện tại
        //    var existingUser = _db.Users.FirstOrDefault(u => u.Username == newUsername && u.UserId != userId);
        //    if (existingUser != null)
        //    {
        //        return -2; // Tên người dùng đã tồn tại
        //    }

        //    // Kiểm tra mật khẩu có đủ dài không
        //    if (newPassword.Length < 8)
        //    {
        //        return -3; // Mật khẩu phải có ít nhất 8 ký tự
        //    }

        //    // Cập nhật thông tin người dùng
        //    user.Username = newUsername;
        //    user.Password = _customPasswordHasher.HashPassword(newPassword); // Hash mật khẩu

        //    // Lưu thay đổi
        //    _db.SaveChanges();

        //    return 1; // Cập nhật thành công
        //}


        public List<string> GetRoles()
        {
            return _db.Roles.Select(r => r.RoleName).ToList();
        }


        public string GetRoleByUserId(int userId)
        {
            var role = (from ur in _db.UserRoles
                        join r in _db.Roles on ur.RoleId equals r.RoleId
                        where ur.UserId == userId
                        select r.RoleName).FirstOrDefault();

            return role; // Đảm bảo rằng giá trị trả về là tên vai trò (string)
        }

        public List<EmployeeViewModel> GetByName(string name)
        {
            var users = _db.Users.Where(x => x.Username.Contains(name)).ToList();
            var result = users.Select(u => new EmployeeViewModel
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                RoleName = (from ur in _db.UserRoles
                            join r in _db.Roles on ur.RoleId equals r.RoleId
                            where ur.UserId == u.UserId
                            select r.RoleName).FirstOrDefault(),
                AccessType = (from ur in _db.UserRoles
                              join rm in _db.RoleMenus on ur.RoleId equals rm.RoleId
                              where ur.UserId == u.UserId
                              select rm.AccessType).FirstOrDefault()
            }).ToList();

            return result;
        }



        public List<EmployeeViewModel> GetAllEmp()
        {
            //var result = (from u in _db.Users
            //              join ur in _db.UserRoles on u.UserId equals ur.UserId
            //              join r in _db.Roles on ur.RoleId equals r.RoleId
            //              join rm in _db.RoleMenus on r.RoleId equals rm.RoleId
            //              select new EmployeeViewModel
            //              {
            //                  UserId = u.UserId,
            //                  Username = u.Username,
            //                  Email = u.Email,
            //                  RoleName = r.RoleName,
            //                  AccessType = rm.AccessType
            //              }).ToList();
            var result = _db.Users.Select(u => new EmployeeViewModel
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                RoleName = (from ur in _db.UserRoles
                            join r in _db.Roles on ur.RoleId equals r.RoleId
                            where ur.UserId == u.UserId
                            select r.RoleName).FirstOrDefault(),
                AccessType = (from ur in _db.UserRoles
                              join rm in _db.RoleMenus on ur.RoleId equals rm.RoleId
                              where ur.UserId == u.UserId
                              select rm.AccessType).FirstOrDefault()
            }).ToList();
            return result;
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, int newRoleId, string newEmail, string newPassword)
        {
            var userRole = await _db.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (userRole == null || user == null)
                return false;
            var roleExists = await _db.Roles.AnyAsync(r => r.RoleId == newRoleId);
            if (!roleExists)
            {
                
                return false;
            }

            if (!string.IsNullOrWhiteSpace(newEmail))
            {
                user.Email = newEmail;
            }

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                var hashedPassword = _customPasswordHasher.HashPassword(newPassword);
                user.Password = hashedPassword;
            }

            _db.UserRoles.Remove(userRole);
            await _db.SaveChangesAsync();

            var newUserRole = new UserRole
            {
                UserId = userId,
                RoleId = newRoleId
            };

            _db.UserRoles.Add(newUserRole);
            await _db.SaveChangesAsync();

            return true;
        }


        public async Task<bool> Delete(int id)
        {
            var obj =await _db.Users.FirstOrDefaultAsync(x => x.UserId == id);

            if (obj != null)
            {
                var userRoles = _db.UserRoles.Where(ur => ur.UserId == id).ToList();
                if (userRoles != null && userRoles.Any())
                {
                    _db.UserRoles.RemoveRange(userRoles);
                }


                _db.Users.Remove(obj);

            await _db.SaveChangesAsync();
            }

            return true;
        }



    }
}
