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

        [HttpGet("{activityDate}", Name = "GetActivitySummaries")]
        public async Task<ActionResult<IEnumerable<ActivitySummary>>> GetActivitySummaries(string activityDate)
        {
            DateOnly requestedDate;
            IList<Activity> activities;
            IList<ActivitySummary> activitySummaries;

            if (!DateOnly.TryParse(activityDate, out requestedDate))
            {
                return BadRequest();
            }

            activities = await _activityRepository.GetActivitiesForDateAsync(requestedDate);
            if (activities is null)
            {
                return Ok(new List<ActivitySummary>());
            }

            activitySummaries = activities.ToActivitySummaries();

            return Ok(activitySummaries);
        }
    }
}