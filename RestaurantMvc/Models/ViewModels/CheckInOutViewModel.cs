namespace RestaurantMvc.Models.ViewModels
{
    public class CheckInOutViewModel
    {
        public Guid BookingId { get; set; }
        public string UserId { get; set; }
        public DateTime CheckInDate { get; set; }
        public TimeSpan CheckInTime { get; set; }

        public TimeSpan? CheckOutTime { get; set; }
        public decimal? GrossAmount { get; set; }
    }
}
