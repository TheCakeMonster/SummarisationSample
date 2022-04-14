using System.ComponentModel.DataAnnotations;

namespace SummarisationSample.OrderService.Library
{
    public class Order
    {
        [Key]
        [Required]
        public int OrderId { get; set; }

        [Required]
        public string OrderRef { get; set; } = string.Empty;

        [Required]
        public string CustomerRef { get; set; } = string.Empty;

        public IList<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        [Required]
        public OrderStatuses OrderStatus { get; set; } = OrderStatuses.New;

    }
}
