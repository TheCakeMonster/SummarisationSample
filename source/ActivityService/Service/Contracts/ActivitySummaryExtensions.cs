using SummarisationSample.ActivityService.Library;

namespace SummarisationSample.ActivityService.Service.Contracts
{

    /// <summary>
    /// Extension methods for the ActivitySummary DTO
    /// </summary>
    public static class ActivitySummaryExtensions
    {

        public static IList<ActivitySummary> ToActivitySummaries(this IList<Activity> activities)
        {
            IList<ActivitySummary> activitySummaries = new List<ActivitySummary>();

            foreach (Activity activity in activities)
            {
                activitySummaries.Add(activity.ToActivitySummary());
            }

            return activitySummaries;
        }

        public static ActivitySummary ToActivitySummary(this Activity activity)
        {
            ActivityType activityType;

            activityType = ActivityTypes.GetActivityType(activity.ActivityTypeId);

            return new ActivitySummary()
            {
                ActivityDate = activity.ActivityDate.ToDateOnlyString(),
                ActivityTypeCode = activityType.ActivityTypeCode,
                ActivityTypeName = activityType.ActivityTypeName,
                Quantity = activity.Quantity
            };
        }

    }
}
