using AutoMapper;
using Company.G03.BLL;
using Company.G03.BLL.Interfaces;
using Company.G03.BLL.Repositories;
using Company.G03.DAL.Data.Contexts;
using Company.G03.DAL.Entities;
using Company.G03.PL.AutoMapper;
using Company.G03.PL.Services;
using Microsoft.AspNetCore.Identity;
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
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // Allowing the DI container to create the instance of UnitOfWork

            builder.Services.AddIdentity<AppUser, IdentityRole>()
                            .AddEntityFrameworkStores<CompanyDbContext>()
                            .AddDefaultTokenProviders();


            builder.Services.AddAutoMapper(typeof(EmployeeProfile));

            //builder.Services.AddAutoMapper(m => m.AddProfile(new EmployeeProfile()));

            builder.Services.AddDbContext<CompanyDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            }); // Allowing the DI container to create the instance of CompanyDbContext

            //Lifetime services
            builder.Services.AddScoped<IScopedServices, ScopedServices>(); // Per request
            builder.Services.AddTransient<ITransientServices, TransientServices>(); // per operation
            builder.Services.AddSingleton<ISingletonServices, SingletonServices>(); // per application


            #region PathChanges

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/SignIn";
                options.LogoutPath = "/Account/SignOut";
            });

            var app = builder.Build(); // Create an instance of the application

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
                {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                }

            app.UseHttpsRedirection(); // To redirect HTTP requests to HTTPS
            app.UseStaticFiles(); // To serve static files like images, CSS, and JavaScript files

            app.UseRouting(); // To enable routing middleware

            app.UseAuthentication(); // To enable authentication middleware
            app.UseAuthorization(); // To enable authorization middleware

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
            }
        }
    }
