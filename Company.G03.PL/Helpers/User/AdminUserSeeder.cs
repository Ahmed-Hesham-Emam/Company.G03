using Company.G03.DAL.Data.Contexts;
using Company.G03.DAL.Entities;
using Company.G03.PL.Dtos;
using Microsoft.AspNetCore.Identity;
using System.Xml.Serialization;

namespace Company.G03.PL.Helpers.User
    {
    public class AdminUserSeeder
        {

        public static async Task SeedAdminUserFromXmlAsync(IServiceProvider services)
            {
            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var context = services.GetRequiredService<CompanyDbContext>();
            var logger = services.GetRequiredService<ILogger<AdminUserSeeder>>();

            var xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Helpers", "User", "AdminUser.xml");

            if (!File.Exists(xmlFilePath))
                {
                logger.LogError($"XML file not found: {xmlFilePath}");
                return;
                }

            AdminUserDto adminUser;
            try
                {
                var serializer = new XmlSerializer(typeof(AdminUserWrapper));
                using var reader = new StreamReader(xmlFilePath);
                var wrapper = (AdminUserWrapper)serializer.Deserialize(reader);

                if (wrapper == null || wrapper.UserDto == null)
                    {
                    logger.LogError("Failed to deserialize AdminUser.xml or UserDto is null.");
                    return;
                    }

                adminUser = wrapper.UserDto;
                }
            catch (Exception ex)
                {
                logger.LogError($"Error reading admin user from XML: {ex.Message}");
                return;
                }

            var existingUser = await userManager.FindByEmailAsync(adminUser.Email);
            if (existingUser == null)
                {

                var newUser = new AppUser
                    {

                    FirstName = adminUser.FirstName,
                    LastName = adminUser.LastName,
                    TermsAndConditions = adminUser.TermsAndConditions,
                    UserName = adminUser.UserName,
                    NormalizedUserName = adminUser.NormalizedUserName,
                    Email = adminUser.Email,
                    NormalizedEmail = adminUser.NormalizedEmail,
                    EmailConfirmed = adminUser.EmailConfirmed,
                    PasswordHash = adminUser.PasswordHash,
                    SecurityStamp = adminUser.SecurityStamp,
                    ConcurrencyStamp = adminUser.ConcurrencyStamp,
                    PhoneNumberConfirmed = adminUser.PhoneNumberConfirmed,
                    TwoFactorEnabled = adminUser.TwoFactorEnabled,
                    LockoutEnabled = adminUser.LockoutEnabled,
                    AccessFailedCount = adminUser.AccessFailedCount

                    };

                var result = await userManager.CreateAsync(newUser);

                const string adminRoleName = "Admin";
                if (!await roleManager.RoleExistsAsync(adminRoleName))
                    {
                    await roleManager.CreateAsync(new IdentityRole(adminRoleName));
                    }

                await userManager.AddToRoleAsync(newUser, adminRoleName);

                }
            else
                {
                logger.LogInformation($"User with email {adminUser.Email} already exists. Skipping creation.");
                }
            }
        }
    }
