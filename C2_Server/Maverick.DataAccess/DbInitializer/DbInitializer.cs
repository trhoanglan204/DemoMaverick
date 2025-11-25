using Maverick.Models.User;
using Maverick.Utility;
using Maverick.DataAccess.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

#pragma warning disable IDE0290

namespace Maverick.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(
            UserManager<AppUserModel> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            ILogger<DbInitializer> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }

                SeedRolesAndUsers();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while migrating the database.");
            }

        }

        private void CreateUserWithRole(AppUserModel user, string password, string role)
        {
            var result = _userManager.CreateAsync(user, password).GetAwaiter().GetResult();
            if (result.Succeeded)
            {
                _userManager.AddToRoleAsync(user, role).GetAwaiter().GetResult();
                _logger.LogInformation("Created user {Email} with role {Role}", user.Email, role);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError("Failed to create user {Email}: {Error}", user.Email, error.Description);
                }
            }
        }
        private void SeedRolesAndUsers()
        {
            if (_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult()) return;

            foreach (var role in new[] { SD.Role_Employee, SD.Role_Admin })
            {
                _roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
            }
            CreateUserWithRole(new AppUserModel
            {
                UserName = "admin@kma.com",
                Email = "admin@kma.com",
                Name = "AT19_Admin",
                PhoneNumber = "0123456789",
                Role = SD.Role_Admin,
            }, "Admin@123*", SD.Role_Admin);

            CreateUserWithRole(new AppUserModel
            {
                UserName = "staffA",
                Email = "staff@kma.com",
                Name = "AT19_Slave",
                PhoneNumber = "0987654321",
                Role = SD.Role_Employee,
            }, "Staff@123*", SD.Role_Employee);
        }
    }
}
