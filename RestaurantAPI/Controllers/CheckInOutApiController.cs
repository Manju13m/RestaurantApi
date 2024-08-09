using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
            try
            {
                // Check if a check-in record already exists for the given BookingId and UserId
                var existingRecord = await restaurantDbContext.CheckInOuts
                    .FirstOrDefaultAsync(c => c.BookingId == model.BookingId && c.UserId == model.UserId);

                if (existingRecord != null)
                {
                    // Return a conflict status code if the record already exists
                    return Conflict("A check-in record already exists for this booking and user.");
                }

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
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
            {
                // Handle duplicate key error
                return Conflict("A record with the same key already exists.");
            }
            catch (Exception ex)
            {
                // Log and handle the general exception
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPost("Checkout")]
        public async Task<IActionResult> CheckOut([FromBody] CheckInOut model)
        {
            try
            {
                // Check if a check-in record exists for the given BookingId and UserId
                var checkInOut = await restaurantDbContext.CheckInOuts
                    .FirstOrDefaultAsync(c => c.BookingId == model.BookingId && c.UserId == model.UserId && c.CheckInTime != null);

                if (checkInOut == null)
                {
                    return NotFound("No check-in record found for this booking and user. Please check in before checking out.");
                }

                // Check if a check-out record already exists for the same BookingId and UserId
                if (checkInOut.CheckOutTime != null || checkInOut.GrossAmount != null)
                {
                    return Conflict("This booking has already been checked out.");
                }

                // Update the check-out time and gross amount
                checkInOut.CheckOutTime = model.CheckOutTime;
                checkInOut.GrossAmount = model.GrossAmount;

                // Explicitly set the entity state to Modified
                restaurantDbContext.Entry(checkInOut).State = EntityState.Modified;

                // Save changes to the database and check the result
                var result = await restaurantDbContext.SaveChangesAsync();
                if (result == 0)
                {
                    // Log if no records were saved
                    return StatusCode(StatusCodes.Status500InternalServerError, "No records were saved.");
                }

                return Ok(checkInOut); // Return the updated entity to verify changes
            }
            catch (DbUpdateException ex)
            {
                // Handle update exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the check-out record: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }




        [HttpGet("getEmail")]
        public async Task<ActionResult<string>> GetCustomerEmailByUserId([FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            var customer = await restaurantDbContext.Customerdata
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
            {
                return NotFound("Customer not found.");
            }
            //return customer?.Email;

            return Ok(customer.Email); // Assuming the customer object has an Email property
        }
        

    }
}
