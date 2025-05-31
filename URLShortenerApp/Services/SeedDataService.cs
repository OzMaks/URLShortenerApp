using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using URLShortenerApp.Models;

namespace URLShortenerApp.Services
{
    public class SeedDataService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public SeedDataService(RoleManager<IdentityRole> roleManager,
                               UserManager<User> userManager,
                               IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
        }


        public async Task SeedAsync()
        {
            var seedData = _configuration.GetSection("SeedData").Get<SeedDataSettings>();

            foreach (var role in seedData.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));
            }

            foreach (var userData in seedData.Users)
            {
                var user = await _userManager.FindByEmailAsync(userData.Email);
                if (user == null)
                {
                    user = new User
                    {
                        UserName = userData.UserName,
                        Email = userData.Email,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(user, userData.Password);
                    if (result.Succeeded)
                    {
                        foreach (var role in userData.Roles)
                        {
                            await _userManager.AddToRoleAsync(user, role);
                        }
                    }
                }
            }
        }
    }
}
