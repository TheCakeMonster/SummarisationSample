using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummarisationSample.ActivityService.Library
{
    public static class ActivityTypes
    {
        private static ConcurrentDictionary<string, ActivityType> _activityTypes = InitialiseActivityTypes();

        public static ActivityType GetActivityType(int activityTypeId)
        {
            return _activityTypes.FirstOrDefault(at => at.Value.ActivityTypeId == activityTypeId).Value;
        }

        #region Private Helper Methods

        private static ConcurrentDictionary<string, ActivityType> InitialiseActivityTypes()
        {
            ConcurrentDictionary<string, ActivityType> activityTypes = new ConcurrentDictionary<string, ActivityType>();

            AddActivityType(activityTypes, new ActivityType() { ActivityTypeId = 1, ActivityTypeCode = "ORDER_CREATED", ActivityTypeName = "Order Created" });
            AddActivityType(activityTypes, new ActivityType() { ActivityTypeId = 2, ActivityTypeCode = "ORDER_COMPLETED", ActivityTypeName = "Order Completed" });
            AddActivityType(activityTypes, new ActivityType() { ActivityTypeId = 3, ActivityTypeCode = "ORDER_CANCELLED", ActivityTypeName = "Order Cancelled" });

            return activityTypes;
        }
        private static void AddActivityType(ConcurrentDictionary<string, ActivityType> activityTypes, ActivityType activityType)
        {
            activityTypes.TryAdd(activityType.ActivityTypeCode, activityType);
        }

        #endregion
    }
}
