using Company.G03.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace Company.G03.DAL.Data.Contexts
    {
    public class CompanyDbContext : IdentityDbContext<AppUser>
        {
        public CompanyDbContext(DbContextOptions<CompanyDbContext> options) : base(options)
            {

            }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>()
                        .HasIndex(u => u.UserName)
                        .IsUnique(false);

            modelBuilder.Entity<AppUser>()
                        .HasIndex(u => u.NormalizedUserName)
                        .IsUnique(false);
            }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        }
    }
