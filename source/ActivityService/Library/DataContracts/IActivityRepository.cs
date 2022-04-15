using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummarisationSample.ActivityService.Library.DataContracts
{
    public interface IActivityRepository
    {

        Task<IList<Activity>> GetActivitiesForDateAsync(DateOnly activityDate);

        Task<Activity> RecordActivity(IActivityMessage activityMessage);
    }
}
