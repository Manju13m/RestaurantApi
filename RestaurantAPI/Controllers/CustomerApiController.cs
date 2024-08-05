using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models;
using RestaurantAPI.Models.ViewModels;
using System.Security.Claims;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerApiController : Controller
    {
        private readonly RestaurantDbContext restaurantDbContext;

        public CustomerApiController(RestaurantDbContext restaurantDbContext)
        {
            this.restaurantDbContext = restaurantDbContext;
        }

        
        [HttpGet("customer-dashboard-data")]
        public async Task<IActionResult> GetCustomerDashboardData(string userId)
        {
            var today = DateTime.Today;
            var threeDaysLater = today.AddDays(3);

            var upcomingBookings = await restaurantDbContext.Bookingdata
                .Where(b => b.UserId == userId && b.BookingDate >= today && b.BookingDate <= threeDaysLater)
                .Select(b => new AddBookRequest
                {
                    BookingId = b.BookingId,
                    CustomerName = b.CustomerName,
                    BookingDate = b.BookingDate,
                    FromTime = b.FromTime,
                    ToTime = b.ToTime,
                    TableNumber = b.TableNumber,
                    Status = b.Status
                })
                .ToListAsync();

            return Ok(upcomingBookings);
        }
    }

    
    
}
