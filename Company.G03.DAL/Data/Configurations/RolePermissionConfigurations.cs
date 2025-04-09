using Company.G03.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.G03.DAL.Data.Configurations
    {
    public class RolePermissionConfigurations : IEntityTypeConfiguration<RolePermission>
        {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
            {
            builder.HasKey(RP => new { RP.RoleId, RP.PermissionId });

            builder.HasOne(RP => RP.Role)
                .WithMany()
                .HasForeignKey(RP => RP.RoleId);

            builder.HasOne(RP => RP.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(RP => RP.PermissionId);

            }
        }
    }
