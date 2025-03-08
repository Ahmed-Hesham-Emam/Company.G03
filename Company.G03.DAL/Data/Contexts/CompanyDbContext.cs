using Company.G03.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace Company.G03.DAL.Data.Contexts
    {
    public class CompanyDbContext : DbContext
        {
        public CompanyDbContext(DbContextOptions<CompanyDbContext> options) : base(options)
            {

            }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
            }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    {
        //    optionsBuilder.UseSqlServer("Server=.;Database=CompanyG03; Trusted_Connection=True; TrustServerCertificate= True;");
        //    }
        public DbSet<Department> Departments { get; set; }
        }
    }
