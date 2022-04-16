using SummarisationSample.ActivityService.Library;

namespace SummarisationSample.ActivityService.Service.Contracts
{

    /// <summary>
    /// Extension methods for conversion of the NewActivityType class
    /// </summary>
    internal static class NewActivityMessageExtensions
    {

        /// <summary>
        /// Convert the public message into the simple one used internally
        /// </summary>
        /// <param name="message">The message that is to be converted</param>
        /// <returns>The resultant message</returns>
        public static IActivityMessage ToActivityMessage(this NewActivityMessage message)
        {
            ActivityType activityType;
            SimpleActivityMessage activityMessage;

            activityType = ActivityTypes.GetActivityType(message.ActivityTypeCode);

            activityMessage = new SimpleActivityMessage()
            {
                MessageRef = message.MessageRef,
                ActivityTypeId = activityType.ActivityTypeId,
                ActivityAt = message.ActivityAt
            };

            return activityMessage;
        }

    }
}
