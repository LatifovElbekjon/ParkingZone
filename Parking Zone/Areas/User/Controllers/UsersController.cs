using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using ParkingZone.ViewModels.UserVMs;
using System.Collections.Generic;
using ParkingZone.Entities;
using Microsoft.AspNetCore.Authorization;

namespace ParkingZone.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.Select(user => new UserViewModel
            {
                Id = user.Id,
                UserName = user.Name,
                Email = user.UserName,
                Phone = user.PhoneNumber
            }).ToList();

            foreach (var user in users)
            {
                var identityUser = await _userManager.FindByIdAsync(user.Id);
                var roles = await _userManager.GetRolesAsync(identityUser);

                // Assuming the user has only one role
                user.Role = roles.FirstOrDefault() ?? "No Role Assigned";
            }

            return View(users);
        }

        public async Task<IActionResult> Admins()
        {
            //// Check if the "Admin" role exists
            //if (!await _roleManager.RoleExistsAsync("Admin"))
            //{
            //    await _roleManager.CreateAsync(new IdentityRole("Admin"));
            //}
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                return NotFound("Admin role not found.");
            }

            var adminUsers = new List<UserViewModel>();

            // Get all users and check if they are in the Admin role
            foreach (var user in _userManager.Users.ToList())
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    adminUsers.Add(new UserViewModel
                    {
                        Id = user.Id,
                        UserName = user.Name,
                        Email = user.UserName,
                        Phone = user.PhoneNumber
                    });
                }
            }

            foreach (var user in adminUsers)
            {
                var identityUser = await _userManager.FindByIdAsync(user.Id);
                var roles = await _userManager.GetRolesAsync(identityUser);

                // Assuming the user has only one role
                user.Role = roles.FirstOrDefault() ?? "No Role Assigned";
            }

            return View(adminUsers); // Assuming you'll have a corresponding view for admin users
        }
    }
}