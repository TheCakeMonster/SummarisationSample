using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummarisationSample.OrderService.Messaging.Kafka
{
    public class KafkaPublicationFactory : IPublicationFactory
    {
        /// <summary>
        /// Get the configuration for a named publication
        /// </summary>
        /// <param name="publicationName">The name of the publication for which config is required</param>
        /// <returns>The configuration for the publication named</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IPublicationConfiguration GetPublicationConfiguration(string publicationName)
        {
            if (publicationName is null) throw new ArgumentNullException(nameof(publicationName));

            // Only one publication for now; extend if required
            return new KafkaPublicationConfiguration()
            {
                Topic = "summarisation.order"
            };
        }
    }
}
