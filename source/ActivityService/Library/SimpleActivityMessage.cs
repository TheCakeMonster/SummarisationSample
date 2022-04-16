using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummarisationSample.ActivityService.Library
{
    public class SimpleActivityMessage : IActivityMessage
    {
        public string MessageRef { get; set; } = string.Empty;

        public int ActivityTypeId { get; set; }

        public DateTime ActivityAt { get; set; }

    }
}
