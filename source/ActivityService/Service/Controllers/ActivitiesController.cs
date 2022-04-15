using Microsoft.AspNetCore.Mvc;
using SummarisationSample.ActivityService.Library;
using SummarisationSample.ActivityService.Library.DataContracts;
using SummarisationSample.ActivityService.Service.Contracts;

namespace SummarisationSample.ActivityService.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActivitiesController : ControllerBase
    {
        private readonly ILogger<ActivitiesController> _logger;
        private readonly IActivityRepository _activityRepository;

        public ActivitiesController(ILogger<ActivitiesController> logger, IActivityRepository activityRepository)
        {
            _logger = logger;
            _activityRepository = activityRepository;
        }

        [HttpGet(Name = "GetActivitySummaries")]
        public async Task<ActionResult<IEnumerable<ActivitySummary>>> GetActivitySummaries(DateOnly activityDate)
        {
            IList<Activity> activities;
            IList<ActivitySummary> activitySummaries;

            activities = await _activityRepository.GetActivitiesForDateAsync(activityDate);
            if (activities is null)
            {
                return Ok(new List<ActivitySummary>());
            }

            activitySummaries = activities.ToActivitySummaries();

            return Ok(activitySummaries);
        }
    }
}