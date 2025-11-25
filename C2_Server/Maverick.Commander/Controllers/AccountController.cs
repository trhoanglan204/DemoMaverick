using Maverick.DataAccess.Data;
using Maverick.Models;
using Maverick.Models.User;
using Maverick.Models.ViewModels;
using Maverick.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Security.Claims;

#pragma warning disable IDE0290

namespace Maverick.Commander.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly SignInManager<AppUserModel> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<AccountController> _logger;
        public AccountController(UserManager<AppUserModel> userManage,
            SignInManager<AppUserModel> signInManager, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, ILogger<AccountController> logger)
        {
            _userManager = userManage;
            _signInManager = signInManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginVM { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(loginVM.Username!)
                ?? await _userManager.FindByEmailAsync(loginVM.Username!);
                if (user == null)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại.");
                    return View(loginVM);
                }
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user.UserName!, loginVM.Password!, isPersistent: false, lockoutOnFailure: true);
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Tài khoản bị khóa trong 5 phút do đăng nhập sai quá nhiều lần.");
                    return View(loginVM);
                }
                if (result.Succeeded)
                {
                    TempData["success"] = "Đăng nhập thành công";
                    return Redirect(loginVM.ReturnUrl ?? "/");
                }
                ModelState.AddModelError("", "Sai tài khoản hoặc mật khẩu");
            }
            return View(loginVM);
        }
    }
}
