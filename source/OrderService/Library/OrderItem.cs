namespace SummarisationSample.OrderService.Library
{
    public class OrderItem
    {

        public int OrderItemId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public VATRates VATRate { get; set; } = VATRates.Full;

        public decimal VATAmount { get; }

    }
}
