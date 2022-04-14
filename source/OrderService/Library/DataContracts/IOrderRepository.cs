namespace SummarisationSample.OrderService.Library.DataContracts
{
    public interface IOrderRepository
    {

        Task<bool> SaveChangesAsync();

        Task<int> GetNextOrderId();

        Task<Order?> GetOrderAsync(string orderRef);

        Task<IList<Order>> GetOrdersForCustomerAsync(string customerRef);

        Task<IList<Order>> GetOrdersAsync();

        Task PlaceOrderAsync(Order order);

    }
}
