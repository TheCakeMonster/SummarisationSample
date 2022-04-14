using SummarisationSample.OrderService.Library;
using SummarisationSample.OrderService.Library.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummarisationSample.OrderService.Repositories.InMemoryRepository
{

    /// <summary>
    /// In-memory implementation of an Order repository
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private static int _maxOrderId = 0;
        private static IList<Order> _orders = new List<Order>();

        public Task<int> GetNextOrderId()
        {
            int nextOrderId = Interlocked.Increment(ref _maxOrderId);
            return Task.FromResult(nextOrderId);
        }

        public Task<Order?> GetOrderAsync(string orderRef)
        {
            Order? order;

            order = _orders.FirstOrDefault(o => o.OrderRef.Equals(orderRef, StringComparison.CurrentCultureIgnoreCase));
            return Task.FromResult(order);
        }

        public Task<IList<Order>> GetOrdersAsync()
        {
            return Task.FromResult(_orders);
        }

        public Task<IList<Order>> GetOrdersForCustomerAsync(string customerRef)
        {
            IList<Order> orders;

            orders = _orders.Where(o => o.CustomerRef.Equals(customerRef, StringComparison.CurrentCultureIgnoreCase)).ToList();
            return Task.FromResult(_orders);
        }

        public Task PlaceOrderAsync(Order order)
        {
            _orders.Add(order);

            return Task.CompletedTask;
        }

        public Task<bool> SaveChangesAsync()
        {
            return Task.FromResult(true);
        }
    }
}
