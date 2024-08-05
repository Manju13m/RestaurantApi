using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models;

namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckOutApiController : Controller
    {
        private readonly RestaurantDbContext restaurantDbContext;

        public CheckOutApiController(RestaurantDbContext restaurantDbContext)
        {
            this.restaurantDbContext = restaurantDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckOut([FromBody] CheckOut checkOut)
        {
            if (checkOut == null)
            {
                return BadRequest("Invalid CheckOut data.");
            }

            try
            {
                restaurantDbContext.CheckOuts.Add(checkOut);
                await restaurantDbContext.SaveChangesAsync();

                return Ok(checkOut);
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomerEmailById(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                return BadRequest("Customer ID cannot be null or empty.");
            }

            try
            {
                var customer = await restaurantDbContext.Customerdata
            .AsNoTracking()
            .Where(c => c.UserId == customerId)
            .Select(c => new { c.Email })
            .FirstOrDefaultAsync();

                if (customer == null)
                {
                    return NotFound("Customer not found.");
                }

                return Ok(customer.Email);
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
