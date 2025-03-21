using AutoMapper;
using Company.G03.BLL.Interfaces;
using Company.G03.BLL.Repositories;
using Company.G03.DAL.Data.Contexts;
using Company.G03.PL.AutoMapper;
using Company.G03.PL.Services;
using Microsoft.EntityFrameworkCore;

namespace Company.G03.PL
    {
    public class Program
        {
        public static void Main(string[] args)
            {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>(); // Allowing the DI container to create the instance of DepartmentRepository
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>(); // Allowing the DI container to create the instance of EmployeeRepository
            builder.Services.AddAutoMapper(typeof(EmployeeProfile));
            //builder.Services.AddAutoMapper(m => m.AddProfile(new EmployeeProfile()));

            builder.Services.AddDbContext<CompanyDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            }); // Allowing the DI container to create the instance of CompanyDbContext

            //Lifetime
            builder.Services.AddScoped<IScopedServices, ScopedServices>(); // Per request
            builder.Services.AddTransient<ITransientServices, TransientServices>(); // per operation
            builder.Services.AddSingleton<ISingletonServices, SingletonServices>(); // per application

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
                {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); // To serve static files like images, CSS, and JavaScript files

            app.UseRouting();

            //app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Department}/{action=Index}/{id?}");

            app.Run();
            }
        }
    }
