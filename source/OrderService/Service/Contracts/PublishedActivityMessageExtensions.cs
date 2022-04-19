namespace SummarisationSample.OrderService.Service.Contracts
{
    /// <summary>
    /// Extension methods for the PublishedActivityMessage type
    /// </summary>
    internal static class PublishedActivityMessageExtensions
    {

        internal static PublishedActivityMessage ToPublishedActivityMessage(this Library.ActivityMessage activityMessage)
        {
            return new PublishedActivityMessage()
            {
                MessageRef = activityMessage.MessageRef,
                ActivityTypeCode = activityMessage.ActivityTypeCode,
                ActivityAt = activityMessage.ActivityAt,
            };
        }

    }
}
