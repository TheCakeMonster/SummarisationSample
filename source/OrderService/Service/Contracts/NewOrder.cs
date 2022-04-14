namespace SummarisationSample.OrderService.Service.Contracts
{
    public class NewOrder
    {

        public string? CustomerRef { get; set; }

        public IList<NewOrderItem>? OrderItems { get; set; }

    }
}
