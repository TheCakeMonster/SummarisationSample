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
    internal class ActivityRepository : IActivityRepository
    {
        private static readonly HashSet<string> _messageRefs = new HashSet<string>();
        private static readonly ConcurrentDictionary<DateOnly, ConcurrentDictionary<int, Activity>> _activities = 
            new ConcurrentDictionary<DateOnly, ConcurrentDictionary<int, Activity>>();
        private static readonly object _activitiesLock = new object();

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

        public Task<Activity> RecordActivity(IActivityMessage activityMessage)
        {
            Activity activity;
            DateOnly activityDate;
            ConcurrentDictionary<int, Activity>? activitiesDict;

            if (_messageRefs.Contains(activityMessage.MessageRef)) return null;

            activityDate = DateOnly.FromDateTime(activityMessage.ActivityAt);
            activitiesDict = _activities.GetOrAdd(activityDate, (dt) => new ConcurrentDictionary<int, Activity>());
            activity = activitiesDict.GetOrAdd(activityMessage.ActivityTypeId, 
                (dt) => new Activity() { ActivityDate = activityDate, ActivityTypeId = activityMessage.ActivityTypeId, Quantity = 1 });

            return Task.FromResult(activity);
        }
    }
}
