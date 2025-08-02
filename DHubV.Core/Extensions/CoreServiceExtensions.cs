using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DHubV.Core.Extensions
{
    public static class CoreServiceExtensions
    {
        public static IServiceCollection CoreExtension(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
           
        }
    }
}
