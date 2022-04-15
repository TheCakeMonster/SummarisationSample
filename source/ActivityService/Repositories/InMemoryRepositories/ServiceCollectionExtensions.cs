using SummarisationSample.ActivityService.InMemoryRepositories;
using SummarisationSample.ActivityService.Library.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{

    /// <summary>
    /// Extension methods to register types for dependency injection
    /// </summary>
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddInMemoryRepositories(this IServiceCollection services)
        {
            services.AddTransient<IActivityRepository, ActivityRepository>();

            return services;
        }
    }
}
