namespace SummarisationSample.OrderService.Library
{
    public static class OrderStatusExtensions
    {

        public static string ToStatusName(this OrderStatuses orderStatus)
        {
            return orderStatus.ToString();
        }

    }
}
