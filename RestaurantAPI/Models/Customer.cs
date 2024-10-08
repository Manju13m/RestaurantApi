﻿using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Models
{
    public class Customer
    {
        [Key]

        public string UserId { get; set; }

        [Required]
        [StringLength(20)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(20)]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        public string Address { get; set; }



        [Required]
        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits.")]
        public string PhoneNo { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.(com|in|net|org|edu)$", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Password hash and salt fields
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }


        public ICollection<Booking> Bookingdata { get; set; }
    }
}
