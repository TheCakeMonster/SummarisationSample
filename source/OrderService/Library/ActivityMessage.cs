using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummarisationSample.OrderService.Library
{
    public class ActivityMessage
    {
        private int _publishingFailures;

        public string MessageRef { get; set; } = MessageRefGenerator.Generate();

        public string ActivityTypeCode { get; set; } = string.Empty;

        public DateTime ActivityAt { get; set; } = DateTime.Now;

        public DateTime? PublishedAt { get; set; } = null;

        public int PublishingFailures 
        { 
            get { return _publishingFailures; } 
            set { _publishingFailures = value; }
        }

        public void IncrementPublishingFailures()
        {
            Interlocked.Increment(ref _publishingFailures);
        }

    }
}
