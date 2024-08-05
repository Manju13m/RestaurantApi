using System.ComponentModel.DataAnnotations;

namespace RestaurantMvc.Models.ViewModels
{
    public class CheckInViewModel
    {
        [Required]
        public string CustomerId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan CheckInTime { get; set; }
    }
}
