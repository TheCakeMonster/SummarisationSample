using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummarisationSample.ActivityService.Library
{
    public interface IActivityMessage
    {
        string MessageRef { get; set; }

        int ActivityTypeId { get; set; }

        DateTime ActivityAt { get; set; }

    }
}
