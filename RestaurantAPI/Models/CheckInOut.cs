using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Models
{
    public class CheckInOut
    {


        // public Guid id { get; set; }
        [Key]
        public Guid BookingId { get; set; } 
        public string UserId { get; set; } 
        public DateTime? CheckInDate { get; set; } // Nullable to handle the case when not checked in yet
        public TimeSpan? CheckInTime { get; set; } // Nullable to handle the case when not checked in yet
        
        public TimeSpan? CheckOutTime { get; set; } // Nullable to handle the case when not checked out yet
        public decimal? GrossAmount { get; set; }
    }
}
