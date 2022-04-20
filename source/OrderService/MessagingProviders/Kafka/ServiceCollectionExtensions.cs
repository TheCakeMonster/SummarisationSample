using SummarisationSample.OrderService.Messaging.Kafka;
using SummarisationSample.OrderService.Messaging;
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
        /// Do all of the service registration required to use Kafka
        /// </summary>
        /// <param name="services">The instance of IServiceCollection that is being extended</param>
        /// <returns>The IServiceCollection extended, to support method chaining</returns>
        public static IServiceCollection AddKafkaMessaging(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IMessagePublisher<,>), typeof(KafkaMessagePublisher<,>));
            return services;
        }
    }
}
