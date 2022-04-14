using SummarisationSample.OrderService.Library;

namespace SummarisationSample.OrderService.Contracts
{
    /// <summary>
    /// Extension methods for conversion to and from the OrderSummary DTO
    /// </summary>
    internal static class OrderSummaryExtensions
    {

        /// <summary>
        /// Convert an Order object to a new OrderSummary DTO
        /// </summary>
        /// <param name="order">The order we are to convert to a DTO</param>
        /// <returns>An OrderSummary DTO object carrying the summary data of the model</returns>
        internal static OrderSummary ToOrderSummary(this Order order)
        {
            OrderSummary orderSummary;

            orderSummary = new OrderSummary()
            {
                OrderRef = order.OrderRef,
                CustomerRef = order.CustomerRef,
                NumberOfItems = order.OrderItems.Count,
                StatusName = order.OrderStatus.ToStatusName(),
            };

            return orderSummary;
        }

        /// <summary>
        /// Convert a list of Order objects to OrderSummary DTOs
        /// </summary>
        /// <param name="orders">The orders we are to convert to DTOs</param>
        /// <returns>OrderSummary DTO objects carrying the summary data of each model</returns>
        internal static IList<OrderSummary> ToOrderSummaries(this IList<Order> orders)
        {
            IList<OrderSummary> orderSummaries = new List<OrderSummary>();

            foreach (Order order in orders)
            {
                orderSummaries.Add(order.ToOrderSummary());
            }

            return orderSummaries;
        }
    }
}
