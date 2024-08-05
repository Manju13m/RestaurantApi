using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Models
{
    public class CheckOut
    {
        public int CheckOutId { get; set; }
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public decimal GrossAmount { get; set; }
    }
}
