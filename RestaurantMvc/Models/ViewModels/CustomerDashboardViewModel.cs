namespace RestaurantMvc.Models.ViewModels
{
    public class CustomerDashboardViewModel
    {
        public string CustomerName { get; set; }
        public List<AddBookRequest> Bookings { get; set; }
    }
}
