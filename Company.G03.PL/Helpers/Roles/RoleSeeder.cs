using Company.G03.BLL.Enums;
using Company.G03.DAL.Data.Contexts;
using Company.G03.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Company.G03.PL.Helpers.Roles
    {
    public class RoleSeeder
        {

        public static async Task SeedRoleAsync(CompanyDbContext context, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
            {
            string[] RoleNames = { "Admin", "User" };

            foreach (var RoleName in RoleNames)
                {
                var Role = await roleManager.FindByNameAsync(RoleName);
                if (Role == null) // Check if the role already exists if not create it
                    {
                    var newRole = new IdentityRole
                        {
                        Name = RoleName,
                        NormalizedName = RoleName.Normalize().ToUpper(),
                        };
                    await roleManager.CreateAsync(newRole);
                    }
                }

            var allPermissions = Enum.GetNames(typeof(PermissionEnum)); // Get all permission names from the enum

            var DBPermissions = await context.Permissions.Where(p => allPermissions.Contains(p.Name)).ToListAsync(); // Get all permissions from the database


            var admin = await roleManager.FindByNameAsync("Admin"); // Find the admin role in the database
            var user = await roleManager.FindByNameAsync("User"); // Find the user role in the database


            var rolePermissions = await context.RolePermissions
                .Where(rp => rp.RoleId == admin.Id)
                .ToListAsync(); // Get all role permissions for the admin role

            if (admin != null) // Check if the admin role exists
                {
                foreach (var permission in DBPermissions)
                    {
                    bool alreadyExists = rolePermissions.Any(rp => rp.PermissionId == permission.Id); // Check if the permission already exists for the role
                    if (!alreadyExists) // If it doesn't exist, add it
                        {
                        await context.RolePermissions.AddAsync(new RolePermission
                            {
                            RoleId = admin.Id,
                            PermissionId = permission.Id
                            }); // Add the new role permission to the context
                        }
                    }
                }


            if (user != null) // Check if the user role exists
                {
                var userPermissions = DBPermissions.FirstOrDefault(p => p.Name == "View"); // Find the "View" permission
                if (userPermissions != null)
                    {
                    var userRolePermissions = await context.RolePermissions.Where(rp => rp.RoleId == user.Id).ToListAsync(); // Get all role permissions for the user role

                    bool alreadyExists = userRolePermissions.Any(rp => rp.Permission.Name == "View");
                    if (!alreadyExists)

                        if (!alreadyExists) // If it doesn't exist, add it
                            {
                            var permission = await context.Permissions.FirstOrDefaultAsync(p => p.Name == "View"); // Find the permission in the database
                            if (permission != null)
                                {
                                await context.RolePermissions.AddAsync(new RolePermission
                                    {
                                    RoleId = user.Id,
                                    PermissionId = permission.Id
                                    });
                                }
                            }
                    }
                }


            // Assign the "User" role to all users who don't have a role
            var users = await userManager.Users.ToListAsync();
            foreach (var applicationUser in users)
                {
                var userRoles = await userManager.GetRolesAsync(applicationUser);
                if (!userRoles.Any()) // If the user has no role
                    {
                    await userManager.AddToRoleAsync(applicationUser, "User"); // Assign the "User" role
                    }
                }

            await context.SaveChangesAsync(); // Save changes to the database

            }
        }
    }
