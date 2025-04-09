using Company.G03.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.G03.BLL.Interfaces
    {
    public interface IPermissionRepository
        {
        Task<List<Permission>> GetAllPermissionsAsync();
        Task<Permission?> GetPermissionByIdAsync(string id);
        Task<Permission?> GetPermissionByNameAsync(string name);
        Task<List<string>> GetPermissionIdsByRoleIdAsync(string roleId);
        public Task<IdentityRole?> GetRoleByNameAsync(string roleName);
        Task<(List<string> Assigned, List<string> Unassigned)> GetPermissionNamesByRoleAsync(string roleId);
        Task AddPermissionAsync(Permission permission);
        Task UpdatePermissionAsync(Permission permission);
        Task UpdateRolePermissionsAsync(string roleId, List<string> newPermissionNames);
        Task DeletePermissionAsync(string id);

        }
    }
