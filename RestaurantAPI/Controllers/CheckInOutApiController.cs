using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckInOutApiController : Controller
    {

        private readonly RestaurantDbContext restaurantDbContext;

        public CheckInOutApiController(RestaurantDbContext restaurantDbContext)
        {
            this.restaurantDbContext = restaurantDbContext;
        }

        [HttpPost("Checkin")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInOut model)
        {
            var checkInOut = new CheckInOut
            {
                BookingId = model.BookingId,
                UserId = model.UserId,
                CheckInDate = model.CheckInDate,
                CheckInTime = model.CheckInTime
            };

            restaurantDbContext.CheckInOuts.Add(checkInOut);
            await restaurantDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("Checkout")]
        public async Task<IActionResult> CheckOut([FromBody] CheckInOut model)
        {
            var checkInOut = await restaurantDbContext.CheckInOuts
                .FirstOrDefaultAsync(c => c.BookingId == model.BookingId && c.UserId == model.UserId);

            if (checkInOut == null)
            {
                return NotFound();
            }

            checkInOut.CheckOutTime = model.CheckOutTime;
            checkInOut.GrossAmount = model.GrossAmount;

            restaurantDbContext.CheckInOuts.Update(checkInOut);
            await restaurantDbContext.SaveChangesAsync();

            

            return Ok();
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<string>> GetCustomerEmailByUserId(string userId)
        {
            var customer = await restaurantDbContext.Customerdata
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
            {
                return NotFound();
            }

            // Return only the email address
            return Ok(customer.Email);
        }
    }
}
