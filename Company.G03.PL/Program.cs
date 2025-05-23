using AutoMapper;
using Company.G03.BLL;
using Company.G03.BLL.Interfaces;
using Company.G03.BLL.Repositories;
using Company.G03.DAL.Data.Contexts;
using Company.G03.DAL.Entities;
using Company.G03.PL.AutoMapper;
using Company.G03.PL.Helpers.Email;
using Company.G03.PL.Helpers.Permissions;
using Company.G03.PL.Helpers.Roles;
using Company.G03.PL.Helpers.SMS;
using Company.G03.PL.Helpers.User;
using Company.G03.PL.Services;
using Company.G03.PL.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Company.G03.PL
    {
    public class Program
        {
        public static async Task Main(string[] args)
            {
            var builder = WebApplication.CreateBuilder(args);

            #region DI
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>(); // Allowing the DI container to create the instance of DepartmentRepository
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>(); // Allowing the DI container to create the instance of EmployeeRepository
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // Allowing the DI container to create the instance of UnitOfWork
            builder.Services.AddScoped<IPermissionRepository, PermissionRepository>(); // Allowing the DI container to create the instance of PermissionRepository
            builder.Services.AddScoped<IPermissionService, PermissionService>(); // Allowing the DI container to create the instance of PermissionService
            #endregion

            builder.Services.AddAutoMapper(typeof(EmployeeProfile)); // Registering the AutoMapper profile for mapping between DTOs and entities

            #region DB_Connection

            builder.Services.AddDbContext<CompanyDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            #endregion

            #region AppUser
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;// Email will be unique
            });

            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Lockout duration of 5 minutes
                options.Lockout.MaxFailedAccessAttempts = 5; // Maximum failed access attempts before lockout
                // User settings
                //options.User.RequireUniqueEmail = true; // Require unique email addresses for users
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ "; // Allowed characters for usernames
            })
    .AddEntityFrameworkStores<CompanyDbContext>()
    .AddDefaultTokenProviders();

            //builder.Services.ConfigureApplicationCookie(options =>
            //{
            //    options.LoginPath = "/Account/SignIn"; // Redirect to this path if the user is not authenticated
            //    options.ExpireTimeSpan = TimeSpan.FromDays(30); // Set the expiration time for the authentication cookie
            //    options.LogoutPath = "/Account/SignOut"; // Redirect to this path when the user logs out
            //    options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect to this path if access is denied
            //});


            #endregion

            #region Mail

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(nameof(EmailSettings))); // Bind the EmailSettings section of the configuration to the EmailSettings class
            builder.Services.AddScoped<IEmailService, MailService>(); // Allowing the DI container to create the instance of MailService

            #endregion

            #region Twilio

            builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection(nameof(TwilioSettings))); // Bind the TwilioSettings section of the configuration to the TwilioSettings class

            builder.Services.AddScoped<ITwilioService, TwilioService>(); // Allowing the DI container to create the instance of TwilioService

            #endregion

            #region Auth

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/SignIn";
                options.LogoutPath = "/Account/SignOut";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
                options.SlidingExpiration = true;
            });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                //options.LoginPath = "/Account/SignIn"; // Redirect to this path if the user is not authenticated
                //options.LogoutPath = "/Account/SignOut"; // Redirect to this path when the user logs out
                //options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect to this path if access is denied
                //options.ExpireTimeSpan = TimeSpan.FromDays(1); // Set the expiration time for the authentication cookie
                //options.SlidingExpiration = true; // Enable sliding expiration
            }).AddGoogle(options =>
            {
                IConfiguration GoogleAuth = builder.Configuration.GetSection("Auth:Google"); // Get the Google authentication settings from the configuration
                options.ClientId = GoogleAuth["ClientId"];// Google Client ID
                options.ClientSecret = GoogleAuth["ClientSecret"]; // Google Client Secret
            }).AddFacebook(options =>
            {
                IConfiguration FacebookAuth = builder.Configuration.GetSection("Auth:Facebook"); // Get the Facebook authentication settings from the configuration
                options.ClientId = FacebookAuth["ClientId"]; // Facebook Client ID
                options.ClientSecret = FacebookAuth["ClientSecret"]; // Facebook Client Secret
            });

            #endregion

            #region Lifetime services

            builder.Services.AddScoped<IScopedServices, ScopedServices>(); // Per request
            builder.Services.AddTransient<ITransientServices, TransientServices>(); // per operation
            builder.Services.AddSingleton<ISingletonServices, SingletonServices>(); // per application

            #endregion

            #region Old Auth

            //        builder.Services.ConfigureApplicationCookie(options =>
            //        {
            //            options.LoginPath = "/Account/SignIn";
            //            options.LogoutPath = "/Account/SignOut";
            //            options.AccessDeniedPath = "/Account/AccessDenied";
            //        });

            //        #endregion

            //        #region Auth

            //        #region Google
            //        builder.Services.AddAuthentication(o =>
            //{
            //    o.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
            //    o.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            //}).AddGoogle(o =>
            //    {
            //        o.ClientId = builder.Configuration["Auth:Google:ClientId"];
            //        o.ClientSecret = builder.Configuration["Auth:Google:ClientSecret"];
            //    }
            //    );
            //        #endregion

            //        #region Facebook

            //        builder.Services.AddAuthentication(o =>
            //        {
            //            o.DefaultAuthenticateScheme = FacebookDefaults.AuthenticationScheme;
            //            o.DefaultChallengeScheme = FacebookDefaults.AuthenticationScheme;
            //        }).AddFacebook(o =>
            //        {
            //            o.ClientId = builder.Configuration["Auth:Facebook:ClientId"];
            //            o.ClientSecret = builder.Configuration["Auth:Facebook:ClientSecret"];
            //        }
            //);

            //        #endregion

            #endregion

            var app = builder.Build(); // Create an instance of the application

            #region Data Seeders
            using ( var scope = app.Services.CreateScope() )
                {
                var services = scope.ServiceProvider; // Create a scope for the services
                var context = services.GetRequiredService<CompanyDbContext>(); // Get the database context
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>(); // Get the RoleManager service
                var userManager = services.GetRequiredService<UserManager<AppUser>>(); // Get the UserManager service


                await PermissionSeeder.SeedPermissionsAsync(context); // Seed the permissions from the enum
                await RoleSeeder.SeedRoleAsync(context, roleManager, userManager); // Seed the admin role with permissions
                await AdminUserSeeder.SeedAdminUserFromXmlAsync(services); // Seed the admin user from the XML file
                }

            #endregion

            // Configure the HTTP request pipeline.
            if ( !app.Environment.IsDevelopment() )
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
