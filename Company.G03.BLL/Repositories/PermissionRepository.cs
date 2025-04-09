using Company.G03.BLL.Interfaces;
using Company.G03.DAL.Data.Contexts;
using Company.G03.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.G03.BLL.Repositories
    {
    public class PermissionRepository : IPermissionRepository
        {

        private readonly CompanyDbContext _context;

        public PermissionRepository(CompanyDbContext context)
            {
            _context = context;
            }


        public async Task AddPermissionAsync(Permission permission)
            {
            await _context.Permissions.AddAsync(permission);
            await _context.SaveChangesAsync();
            }

        public async Task DeletePermissionAsync(string id)
            {
            var permission = _context.Permissions.FirstOrDefaultAsync(p => p.Id == id);
            if (permission != null)
                {
                _context.Permissions.Remove(permission.Result);
                await _context.SaveChangesAsync();
                }
            }

        public async Task<List<Permission>> GetAllPermissionsAsync()
            {
            return await _context.Permissions.ToListAsync();
            }

        public Task<Permission?> GetPermissionByIdAsync(string id)
            {
            return _context.Permissions.FirstOrDefaultAsync(p => p.Id == id);
            }

        public Task<Permission?> GetPermissionByNameAsync(string name)
            {
            return _context.Permissions.FirstOrDefaultAsync(p => p.Name == name);
            }

        public async Task<List<string>> GetPermissionIdsByRoleIdAsync(string roleId)
            {
            return await _context.RolePermissions.Where(rp => rp.RoleId == roleId)
                                                    .Select(rp => rp.PermissionId)
                                                    .ToListAsync();
            }

        public async Task<(List<string> Assigned, List<string> Unassigned)> GetPermissionNamesByRoleAsync(string roleId)
            {
            var allPermissions = await _context.Permissions.ToListAsync();

            var rolePermissionIds = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            var assigned = allPermissions
                .Where(p => rolePermissionIds.Contains(p.Id))
                .Select(p => p.Name)
                .ToList();

            var unassigned = allPermissions
                .Where(p => !rolePermissionIds.Contains(p.Id))
                .Select(p => p.Name)
                .ToList();

            return (assigned, unassigned);
            }

        public async Task<IdentityRole?> GetRoleByNameAsync(string roleName)
            {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            }

        public async Task UpdatePermissionAsync(Permission permission)
            {
            _context.Permissions.Update(permission);
            await _context.SaveChangesAsync();
            }

        public async Task UpdateRolePermissionsAsync(string roleId, List<string> newPermissionNames)
            {
            var rolePermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            var existingPermissionIds = rolePermissions.Select(rp => rp.PermissionId).ToList();

            var allPermissions = await _context.Permissions.ToListAsync();

            var newPermissionIds = allPermissions
                .Where(p => newPermissionNames.Contains(p.Name))
                .Select(p => p.Id)
                .ToList();

            // Remove old
            var toRemove = rolePermissions
                .Where(rp => !newPermissionIds.Contains(rp.PermissionId))
                .ToList();
            _context.RolePermissions.RemoveRange(toRemove);

            // Add new
            var toAdd = newPermissionIds
                .Where(pid => !existingPermissionIds.Contains(pid))
                .Select(pid => new RolePermission { RoleId = roleId, PermissionId = pid })
                .ToList();

            _context.RolePermissions.AddRange(toAdd);

            await _context.SaveChangesAsync();
            }

        }
    }
