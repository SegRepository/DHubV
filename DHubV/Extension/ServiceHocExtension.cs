using DHubV.Application.Services.UserAuth;

namespace DHubV_.Extension
{
    public static class ServiceHocExtension
    {
        public static IServiceCollection AddServiceDI(this IServiceCollection services)
        {
            services.AddScoped<IUserAuth, UserAuthService>();
            return services; 
        }
    }
}
