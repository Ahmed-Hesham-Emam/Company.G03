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
    public class EmployeeConfigurations : IEntityTypeConfiguration<Employee>
        {
        public void Configure(EntityTypeBuilder<Employee> builder)
            {
            builder.Property(e => e.Salary).HasColumnType("decimal(18,2)");

            builder.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);
            }
        }
    }
