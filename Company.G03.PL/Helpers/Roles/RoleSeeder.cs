using Company.G03.BLL.Enums;
using Company.G03.DAL.Data.Contexts;
using Company.G03.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Company.G03.PL.Helpers.Roles
    {
    public class RoleSeeder
        {
        public static async Task SeedAdminRoleAsync(CompanyDbContext context, RoleManager<IdentityRole> roleManager)
            {
            string adminRoleName = "Admin";
            var adminRole = await roleManager.FindByNameAsync(adminRoleName);
            if (adminRole == null) // Check if the role already exists if not create it
                {
                var role = new IdentityRole
                    {
                    Name = adminRoleName,
                    NormalizedName = adminRoleName.ToUpper()
                    };
                await roleManager.CreateAsync(role);
                }


            var allPermissions = Enum.GetNames(typeof(PermissionEnum)); // Get all permission names from the enum

            var DBPermissions = await context.Permissions.Where(p => allPermissions.Contains(p.Name)).ToListAsync(); // Get all permissions from the database

            var rolePermissions = await context.RolePermissions.Where(rp => rp.RoleId == adminRole.Id)
                .ToListAsync(); // Get all role permissions for the admin role

            foreach (var permission in DBPermissions)
                {
                bool alreadyExists = rolePermissions.Any(rp => rp.PermissionId == permission.Id); // Check if the permission already exists for the role
                if (!alreadyExists) // If it doesn't exist, add it
                    {

                    await context.RolePermissions.AddAsync(new RolePermission
                        {
                        RoleId = adminRole.Id,
                        PermissionId = permission.Id
                        }); // Add the new role permission to the context

                    }
                }
            await context.SaveChangesAsync(); // Save changes to the database

            }
        }
    }
