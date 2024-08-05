using System.ComponentModel.DataAnnotations;

namespace RestaurantMvc.Models.ViewModels
{
    public class CheckOutViewModel
    {
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public decimal GrossAmount { get; set; }
    }
}
