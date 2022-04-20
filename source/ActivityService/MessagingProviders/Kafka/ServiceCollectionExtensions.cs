using SummarisationSample.ActivityService.Messaging;
using SummarisationSample.ActivityService.Messaging.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Perform all of the required service registration for use of Kafka
        /// </summary>
        /// <param name="services">The instance of the IServiceCollection which this method extends</param>
        /// <returns>The IServiceCollection provided, to support method chaining</returns>
        public static IServiceCollection AddKafkaMessaging(this IServiceCollection services)
        {
            services.AddTransient(typeof(IMessageReceiver<,>), typeof(KafkaMessageReceiver<,>));
            services.AddTransient<ISubscriptionFactory, KafkaSubscriptionFactory>();

            return services;
        }
    }
}
