using Company.G03.BLL.Enums;
using Company.G03.DAL.Data.Contexts;
using Company.G03.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Company.G03.PL.Helpers.Permissions
    {
    public class PermissionSeeder
        {

        public static async Task SeedPermissionsAsync(CompanyDbContext context)
            {
            var enumPermissions = Enum.GetNames(typeof(PermissionEnum));
            var existingPermissions = await context.Permissions.Select(p => p.Name).ToListAsync();

            foreach (var permission in enumPermissions)
                {
                if (!existingPermissions.Contains(permission))
                    {
                    context.Permissions.Add(new Permission { Name = permission });
                    }
                }

            await context.SaveChangesAsync();
            }

        }
    }
