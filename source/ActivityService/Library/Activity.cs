using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummarisationSample.ActivityService.Library
{
    public class Activity
    {
        [Key]
        public int ActivityId { get; set; }

        [Required]
        public DateOnly ActivityDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Required]
        public int ActivityTypeId { get; set; }

        public int Quantity { get; set; }
    }
}
