using SummarisationSample.OrderService.Library;

namespace SummarisationSample.OrderService.Contracts
{
    /// <summary>
    /// Extension methods for conversion to and from the NewOrderItem DTO
    /// </summary>
    internal static class NewOrderItemExtensions
    {

        /// <summary>
        /// Convert a NewOrderItem DTO to a new OrderItem model instance
        /// </summary>
        /// <param name="newOrderItem">The parameters for the new order item</param>
        /// <returns>An OrderItem object carrying the parameters from the DTO</returns>
        internal static OrderItem ToOrderItem(this NewOrderItem newOrderItem)
        {
            OrderItem orderItem;

            orderItem = new OrderItem()
            {
                ProductId = newOrderItem.ProductId,
                Quantity = newOrderItem.Quantity,
            };

            return orderItem;
        }

    }
}
