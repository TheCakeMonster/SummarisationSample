using Microsoft.Extensions.DependencyInjection;
using SummarisationSample.OrderService.Library.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummarisationSample.OrderService.Repositories.InMemoryRepository
{

    /// <summary>
    /// Extension methods for the IServiceCollection type
    /// </summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Register the in-memory repositories 
        /// </summary>
        /// <param name="services">The services collection which is being extended</param>
        /// <returns>The extended service collection, to enable method chaining</returns>
        public static IServiceCollection AddInMemoryRepositories(this IServiceCollection services)
        {
            services.AddTransient<IOrderRepository, OrderRepository>();

            return services;
        }
    }
}
