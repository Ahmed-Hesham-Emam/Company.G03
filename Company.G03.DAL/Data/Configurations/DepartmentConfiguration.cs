using Company.G03.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Company.G03.DAL.Data.Configurations
    {
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
        {
        public void Configure(EntityTypeBuilder<Department> builder)
            {
            builder.Property(propertyExpression: d => d.Id).UseIdentityColumn(10, 10);
            }
        }
    }
