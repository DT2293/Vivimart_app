using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Services.SystemService;
using WebApplication1.ViewModels;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AuthContext _db;
        private readonly SystemService _systemService;
        private readonly CustomPasswordHasher _customPasswordHasher;
        public HomeController(AuthContext db, ILogger<HomeController> logger, SystemService systemService, CustomPasswordHasher customPasswordHasher)
        {
            _db = db;
            _logger = logger;
            _systemService = systemService;
            _customPasswordHasher = customPasswordHasher;
        }
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var username = HttpContext.Session.GetString("Username");
            var userInfo = await _db.Users
                .Where(u => u.Username == username)
                .Select(u => new
                {
                    u.Username,
                    u.Email,
                    RoleName = _db.UserRoles
                        .Where(ur => ur.UserId == u.UserId)
                        .Join(_db.Roles, ur => ur.RoleId, r => r.RoleId, (ur, r) => r.RoleName)
                        .FirstOrDefault(),
                    AccessType = _db.RoleMenus
                        .Where(rm => rm.RoleId == _db.UserRoles
                            .Where(ur => ur.UserId == u.UserId)
                            .Select(ur => ur.RoleId)
                            .FirstOrDefault())
                        .Select(rm => rm.AccessType)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (userInfo == null)
            {
                return NotFound();
            }
            ViewBag.Username = userInfo.Username;
            ViewBag.Email = userInfo.Email;
            ViewBag.RoleName = userInfo.RoleName;
            ViewBag.AccessType = userInfo.AccessType;
           
            return View("Index");
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            var viewModel = new CreateUserViewModel
            {
                User = new User(),
                Roles = _systemService.GetRoles()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateUser(CreateUserViewModel model)
        {
            var kt = _systemService.AddUserWithRoleAsync(model.User.Username, model.User.Password, model.User.Email, model.SelectedRole);
            if (kt == 0)
            {

                return RedirectToAction("Index");

            }
            else if (kt == 2)
            {
                model.Error.Add("tai khoan da ton tai");
                model.Roles = _systemService.GetRoles();
                return View(model);
            }
            else
            {
                model.Roles = _systemService.GetRoles();
                model.Error.Add("mat khau chua du 8 ky tu");
                return View(model);
            }
        }

        [HttpGet]     
        public IActionResult ShowAllEmp(EmpoyeeViewModel data)
        {
            if (!string.IsNullOrEmpty(data.keyWord))
            {

                data.employeeViewModels = _systemService.GetByName(data.keyWord);
            }
            else
            {
                data.employeeViewModels = _systemService.GetAllEmp();
            }

            return View(data);
        }


        [HttpGet]
        public IActionResult UpdateRole(int userId)
        {
            var user = _db.Users.FirstOrDefault(u => u.UserId == userId);
            List<string> currentRole = _db.UserRoles.Where(ur => ur.UserId == userId).Select(x => x.RoleId.ToString()).ToList();
            if (user == null)
            {
                return NotFound();
            }

            var model = new UpdateUserRoleViewModel
            {

                UserId = user.UserId,
                Username = user.Username,
                CurrentRole = currentRole,
                NewEmail = user.Email,
                NewRoleId = currentRole[0],
                AvailableRoles = _db.Roles.ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(int userId, int newRoleId, string newEmail, string newPassword)
        {
            bool isUpdated = await _systemService.UpdateUserRoleAsync(userId, newRoleId, newEmail, newPassword);

            if (isUpdated)
            {
                return RedirectToAction("ShowAllEmp");
            }

            ModelState.AddModelError("", "Cập nhật vai trò không thành công.");
            return View();
        }
        [HttpGet]
        public IActionResult UpdatePassword(int userId)
        {
            var user = _db.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                ModelState.AddModelError("", "Người dùng không tồn tại.");
                return View(new UpdateAccountViewModel());
            }

            var model = new UpdateAccountViewModel
            {
                Username = user.Username
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdatePassword(UpdateAccountViewModel model)
        {
            string errorMessage;
            bool isUpdated = _systemService.UpdatePassword(model, out errorMessage);

            if (!isUpdated)
            {
                ModelState.AddModelError("", errorMessage);
                return View(model);
            }

            return RedirectToAction("ShowAllEmp");


        }




        public async Task<IActionResult> Delete(int id)
        {
           await _systemService.Delete(id);

            return RedirectToAction("ShowAllEmp");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

