using DHubV.Application.Helper;
using DHubV.Core.Entity.UserAuth;
using DHubV.Dal.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DHubV_.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDHubVServices(this IServiceCollection services, IConfiguration configuration)
        {

            // Register DbContext with options

            services.AddDbContext<DVHubDBContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                   .EnableSensitiveDataLogging());


            services
              .AddIdentity<User, IdentityRole>(options =>
              {
                  options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                  options.Lockout.MaxFailedAccessAttempts = 5;
                  options.Lockout.AllowedForNewUsers = true;
                  options.Password.RequireDigit = true;
                  options.Password.RequireLowercase = true;
                  options.Password.RequireUppercase = true;
                  options.Password.RequireNonAlphanumeric = true;
                  options.Password.RequiredLength = 8;

                  options.User.RequireUniqueEmail = true;
              })
              .AddEntityFrameworkStores<DVHubDBContext>()
              .AddDefaultTokenProviders();
               services.Configure<JWTConfig>(option => configuration.GetSection("JWT").Bind(option));
            return services;
        }
    }
}
