using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummarisationSample.OrderService.Messaging
{
    public interface IPublicationFactory
    {
        IPublicationConfiguration GetPublicationConfiguration(string publicationName);
    }
}
