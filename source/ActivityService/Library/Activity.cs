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
        private int _quantity;
        [Key]
        public int ActivityId { get; set; }

        [Required]
        public DateOnly ActivityDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Required]
        public int ActivityTypeId { get; set; }

        public int Quantity 
        { 
            get { return _quantity; } 
            set { _quantity = value; } 
        }

        public void IncrementQuantity()
        { 
            Interlocked.Increment(ref _quantity);
        }
    }
}
