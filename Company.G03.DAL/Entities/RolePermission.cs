using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.G03.DAL.Entities
    {
    public class RolePermission
        {
        public string RoleId { get; set; }
        public IdentityRole Role { get; set; }

        public string PermissionId { get; set; }
        public Permission Permission { get; set; }
        }
    }
