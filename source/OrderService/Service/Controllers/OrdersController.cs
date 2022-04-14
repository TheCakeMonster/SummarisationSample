using Microsoft.AspNetCore.Mvc;
using SummarisationSample.OrderService.Service.Contracts;
using SummarisationSample.OrderService.Library.DataContracts;
using SummarisationSample.OrderService.Library;

namespace SummarisationSample.OrderService.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly OrderRefGenerator _orderRefGenerator;
        private readonly IOrderRepository _orderRepository;

        public OrdersController(ILogger<OrdersController> logger, OrderRefGenerator orderRefGenerator, IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRefGenerator = orderRefGenerator;
            _orderRepository = orderRepository;
        }

        [HttpGet(Name = "GetOrders")]
        public async Task<ActionResult<IEnumerable<OrderSummary>>> GetOrders()
        {
            IList<Order> orders;
            IList<OrderSummary> orderSummaries;

            orders = await _orderRepository.GetOrdersAsync();
            orderSummaries = orders.ToOrderSummaries();

            return Ok(orderSummaries);
        }

        [HttpGet("customer/{customerRef}", Name = "GetOrdersForCustomer")]
        public async Task<ActionResult<IEnumerable<OrderSummary>>> GetOrdersForCustomer(string customerRef)
        {
            IList<OrderSummary> orderSummaries;
            IList<Order> orders;

            orders = await _orderRepository.GetOrdersForCustomerAsync(customerRef);
            orderSummaries = orders.ToOrderSummaries();

            return Ok(orderSummaries);
        }

        [HttpGet("{orderRef}", Name = "GetOrder")]
        public async Task<ActionResult<OrderSummary>> GetOrder(string orderRef)
        {
            Order? order = null;
            OrderSummary orderSummary;

            order = await _orderRepository.GetOrderAsync(orderRef);
            if (order is null)
            {
                return NotFound();
            }

            orderSummary = order.ToOrderSummary();

            return Ok(orderSummary);
        }

        [HttpPost]
        public async Task<ActionResult<OrderSummary>> PlaceOrder(NewOrder newOrder)
        {
            Order order;
            OrderSummary orderSummary;

            order = newOrder.ToOrder();
            order.OrderRef = await _orderRefGenerator.GenerateAsync();

            await _orderRepository.PlaceOrderAsync(order);

            orderSummary = order.ToOrderSummary();

            var routeValues = new { orderRef = orderSummary.OrderRef };
            return CreatedAtAction(nameof(GetOrder), routeValues, orderSummary);
        }
    }
}