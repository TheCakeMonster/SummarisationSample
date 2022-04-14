using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummarisationSample.OrderService.Library
{

    /// <summary>
    /// Extension methods for the IServiceCollection type
    /// </summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Register the services required for model behaviours
        /// </summary>
        /// <param name="services">The services collection which is being extended</param>
        /// <returns>The extended service collection, to enable method chaining</returns>
        public static IServiceCollection AddModelServices(this IServiceCollection services)
        {
            services.AddTransient<OrderRefGenerator>();

            return services;
        }
    }
}
