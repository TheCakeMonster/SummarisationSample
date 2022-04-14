using SummarisationSample.OrderService.Library;

namespace SummarisationSample.OrderService.Service.Contracts
{

    /// <summary>
    /// Extension methods for conversion to and from the NewOrder DTO
    /// </summary>
    internal static class NewOrderExtensions
    {

        /// <summary>
        /// Convert a NewOrder DTO to a new Order model instance
        /// </summary>
        /// <param name="newOrder">The parameters for the new order</param>
        /// <returns>An Order object carrying the parameters from the DTO</returns>
        internal static Order ToOrder(this NewOrder newOrder)
        {
            Order order;

            order = new Order()
            {
                OrderRef = string.Empty,
                CustomerRef = newOrder.CustomerRef ?? string.Empty,
                OrderStatus = OrderStatuses.New
            };

            if (newOrder.OrderItems is not null)
            {
                foreach (NewOrderItem newOrderItem in newOrder.OrderItems)
                {
                    order.OrderItems.Add(newOrderItem.ToOrderItem());
                }
            }

            return order;
        }

    }
}
