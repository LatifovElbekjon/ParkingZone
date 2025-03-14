using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using ParkingZone.Entities;
using ParkingZone.Helpers;
using System.ComponentModel.DataAnnotations;

namespace ParkingZone.Areas.Identity.Pages.Account
{
    public class VerifyCodeModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly EEmailService _eEmailService;

        public VerifyCodeModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, ILogger<RegisterModel> logger, IMemoryCache memoryCache, EEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _memoryCache = memoryCache;
            _eEmailService = emailService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string Code { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(Guid userId, string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            // Check if the cache contains the user information
            if (_memoryCache.TryGetValue(userId, out UserCasheModel casheuser))
            {
                if (Input.Code == casheuser.VertifyCode)
                {
                    // Create a new user instance
                    var user = CreateUser();
                    user.Name = casheuser.Name;
                    user.PhoneNumber = casheuser.Phone;
                    user.UserName = casheuser.Email;

                    // Create the user with the provided password
                    var result = await _userManager.CreateAsync(user, casheuser.Password);

                    if (result.Succeeded)
                    {
                        // Ensure the "User" role exists
                        if (!await _roleManager.RoleExistsAsync("User"))
                        {
                            await _roleManager.CreateAsync(new IdentityRole("User"));
                        }

                        // Assign the user to the "User" role
                        var roleResult = await _userManager.AddToRoleAsync(user, "User");
                        if (!roleResult.Succeeded)
                        {
                            // Handle role assignment errors
                            foreach (var error in roleResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return Page();
                        }

                        // Sign in the user
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }

                    // Handle user creation errors
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError("Code", "Invalid verification code");
                    return Page();
                }
            }
            else
            {
                // Handle case where user is not found in the cache
                ModelState.AddModelError("", "Please restart registration");
            }

            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
    }
}

