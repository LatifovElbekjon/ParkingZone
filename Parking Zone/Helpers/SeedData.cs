using Microsoft.AspNetCore.Identity;
using ParkingZone.Entities;

namespace ParkingZone.Helpers
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string adminEmail = "latifovelbek02@gmail.com";
            string adminPassword = "Asd123&";
            string adminRole = "Admin";

            // Admin rolini yaratish agar mavjud bo'lmasa
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            // Admin foydalanuvchini yaratish agar mavjud bo'lmasa
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    Name = "Elbek",
                    PhoneNumber = "+998947030820",
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(newAdmin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, adminRole);
                }
            }
        }
    }

}
