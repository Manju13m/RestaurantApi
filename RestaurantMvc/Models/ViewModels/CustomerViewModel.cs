namespace RestaurantMvc.Models.ViewModels
{
    public class CustomerViewModel
    {
        public string UserId { get; set; } // Assuming UserId is a string (GUID or other identifier)
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
    }
}
