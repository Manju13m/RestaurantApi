namespace RestaurantMvc.Models.ViewModels
{
    public class AdminDashboardData
    {
        public string AdminName { get; set; }
        public int TotalCustomersLast7Days { get; set; }
        public int TotalBookingsNext3Days { get; set; }
        public int TotalCancellationsLast3Days { get; set; }
        public List<AddBookRequest> UpcomingBookings { get; set; }
    }
}
