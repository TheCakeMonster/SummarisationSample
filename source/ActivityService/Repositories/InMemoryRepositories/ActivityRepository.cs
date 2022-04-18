using SummarisationSample.ActivityService.Library;
using SummarisationSample.ActivityService.Library.DataContracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummarisationSample.ActivityService.InMemoryRepositories
{

    /// <summary>
    /// In-memory implementation of the Activity Repository
    /// </summary>
    internal class ActivityRepository : IActivityRepository
    {
        private static readonly HashSet<string> _messageRefs = new HashSet<string>();
        private static readonly ConcurrentDictionary<DateOnly, ConcurrentDictionary<int, Activity>> _activities = 
            new ConcurrentDictionary<DateOnly, ConcurrentDictionary<int, Activity>>();

        /// <summary>
        /// Get the summaries for all activities performed for a specific date
        /// </summary>
        /// <param name="activityDate">The date for which activities are being requested</param>
        /// <returns>IList<Activity> for the date in question</returns>
        public Task<IList<Activity>> GetActivitiesForDateAsync(DateOnly activityDate)
        {
            IList<Activity> activities = new List<Activity>();
            ConcurrentDictionary<int, Activity>? activitiesDict;

            if (_activities.TryGetValue(activityDate, out activitiesDict))
            {
                foreach (Activity activity in activitiesDict.Values)
                {
                    activities.Add(activity);
                }
            }

            return Task.FromResult(activities);
        }

        /// <summary>
        /// Record that an activity occurred, with rejection of duplicate messages
        /// </summary>
        /// <param name="activityMessage">The message we are to handle</param>
        public Task RecordActivity(IActivityMessage activityMessage)
        {
            Activity? activity;
            DateOnly activityDate;
            ConcurrentDictionary<int, Activity>? activitiesDict;

            // Check for previous handling of message - IdEmpotence
            if (_messageRefs.Contains(activityMessage.MessageRef)) return Task.CompletedTask;

            // Find (or create) and incerement the activity by date and activity type
            activityDate = DateOnly.FromDateTime(activityMessage.ActivityAt);
            activitiesDict = _activities.GetOrAdd(activityDate, (dt) => new ConcurrentDictionary<int, Activity>());
            activity = activitiesDict.GetOrAdd(activityMessage.ActivityTypeId, 
                (dt) => new Activity() { ActivityDate = activityDate, ActivityTypeId = activityMessage.ActivityTypeId, Quantity = 0 });
            activity.IncrementQuantity();

            // Mark message as handled - Idempotence
            _messageRefs.Add(activityMessage.MessageRef);

            return Task.CompletedTask;
        }
    }
}
