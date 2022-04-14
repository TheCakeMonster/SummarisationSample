using SummarisationSample.OrderService.Library.DataContracts;

namespace SummarisationSample.OrderService.Library
{
    public class OrderRefGenerator
    {
        private readonly IOrderRepository _orderRepository;

        public OrderRefGenerator(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// Generate a semi-unique OrderRef for an order
        /// </summary>
        /// <returns>A semi-incrementing order reference</returns>
        /// <remarks>
        /// </remarks>
        public async Task<string> GenerateAsync()
        {
            int orderId;
            string orderRef;

            orderId = await _orderRepository.GetNextOrderId();
            orderRef = $"OS{orderId:#####000000}-ORD";

            return orderRef;
        }

        #region Private Helper Methods

        #endregion
    }
}