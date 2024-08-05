using System.ComponentModel.DataAnnotations;


namespace RestaurantMvc.Models.ViewModels
{

    public enum BookingStatus
    {
        Confirmed,
        Cancelled
    }
    public class AddBookRequest
    {
        public string CustomerName { get; set; }

        public Guid BookingId { get; set; }
        public string UserId { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public int TableNumber { get; set; }
        public BookingStatus Status { get; set; }
    } 
}
