using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummarisationSample.ActivityService.Library
{
    public class ActivityType
    {
        [Key]
        [Required]
        public int ActivityTypeId { get; set; }

        [Required]
        public string ActivityTypeCode { get; set; } = string.Empty;

        [Required]
        public string ActivityTypeName { get; set; } = string.Empty;
    }
}
