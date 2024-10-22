using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services.Account;

namespace WebApplication1.Controllers.Account
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
            _authService = authService;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var userInfo = await _authService.LoginAsync(username, password);

            if (userInfo == null)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            HttpContext.Session.SetString("Username", userInfo.Username);
            HttpContext.Session.SetString("RoleName", userInfo.RoleName);
            if (userInfo.RoleName == "System")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Employee", "Employee");
            }
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
