using AutoMapper;
using Company.G03.BLL;
using Company.G03.BLL.Interfaces;
using Company.G03.BLL.Repositories;
using Company.G03.DAL.Data.Contexts;
using Company.G03.DAL.Entities;
using Company.G03.PL.AutoMapper;
using Company.G03.PL.Helpers.Email;
using Company.G03.PL.Helpers.SMS;
using Company.G03.PL.Services;
using Company.G03.PL.Settings;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Company.G03.PL
    {
    public class Program
        {
        public static void Main(string[] args)
            {
            var builder = WebApplication.CreateBuilder(args);

            #region Models
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>(); // Allowing the DI container to create the instance of DepartmentRepository
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>(); // Allowing the DI container to create the instance of EmployeeRepository
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // Allowing the DI container to create the instance of UnitOfWork

            builder.Services.AddIdentity<AppUser, IdentityRole>()
                            .AddEntityFrameworkStores<CompanyDbContext>()
                            .AddDefaultTokenProviders();

            builder.Services.AddAutoMapper(typeof(EmployeeProfile));

            #endregion

            #region Mail

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(nameof(EmailSettings))); // Bind the EmailSettings section of the configuration to the EmailSettings class
            builder.Services.AddScoped<IEmailService, MailService>(); // Allowing the DI container to create the instance of MailService

            #endregion

            #region Twilio

            builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection(nameof(TwilioSettings))); // Bind the TwilioSettings section of the configuration to the TwilioSettings class

            builder.Services.AddScoped<ITwilioService, TwilioService>(); // Allowing the DI container to create the instance of TwilioService

            #endregion

            #region DB_Connection

            builder.Services.AddDbContext<CompanyDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            }); // Allowing the DI container to create the instance of CompanyDbContext

            #endregion

            #region Lifetime services

            builder.Services.AddScoped<IScopedServices, ScopedServices>(); // Per request
            builder.Services.AddTransient<ITransientServices, TransientServices>(); // per operation
            builder.Services.AddSingleton<ISingletonServices, SingletonServices>(); // per application

            #endregion

            #region PathChanges

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/SignIn";
                options.LogoutPath = "/Account/SignOut";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            #endregion

            #region Auth

            #region Google
            builder.Services.AddAuthentication(o =>
    {
        o.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    }).AddGoogle(o =>
        {
            o.ClientId = builder.Configuration["Auth:Google:ClientId"];
            o.ClientSecret = builder.Configuration["Auth:Google:ClientSecret"];
        }
        );
            #endregion

            #region Facebook

            builder.Services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = FacebookDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = FacebookDefaults.AuthenticationScheme;
            }).AddFacebook(o =>
            {
                o.ClientId = builder.Configuration["Auth:Facebook:ClientId"];
                o.ClientSecret = builder.Configuration["Auth:Facebook:ClientSecret"];
            }
    );

            #endregion

            #endregion
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
