using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.email;
using RestaurantAPI.Models;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckInOutApiController : Controller
    {
        private readonly RestaurantDbContext restaurantDbContext;
        private readonly IEmailService emailService;


        public CheckInOutApiController(RestaurantDbContext restaurantDbContext, IEmailService emailService)
        {
            this.restaurantDbContext = restaurantDbContext;
            this.emailService = emailService;
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
                /*

                // Get the customer's email address based on the UserId
                var customer = await restaurantDbContext.Customerdata
                    .FirstOrDefaultAsync(c => c.UserId == model.UserId);

                if (customer == null || string.IsNullOrEmpty(customer.Email))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Customer email not found.");
                }

                // Send an email notification to the customer
                try
                {
                    var subject = "Booking Checkout Confirmation";
                    var message = $"Dear {customer.FirstName},\n\nThank you for dining with us! \n\nYour gross amount for the recent visit is ₹{model.GrossAmount:N0}. \n\nWe appreciate your patronage. \n\nBest regards, \nTrupthi Restaurant.";

                    await emailService.SendEmailAsync(customer.Email, subject, message);
                }
                catch (Exception emailEx)
                {
                    // Log the email error (optional) and return an error response
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Check-out was successful, but an error occurred while sending the email: {emailEx.Message}");
                }

                */

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




        /*  [HttpGet("getEmail")]
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

              return Ok(customer.Email); 
          }*/

        [HttpGet("getEmail")]
        public async Task<IActionResult> GetCustomerEmailByUserId([FromQuery] string userId)
        {
            var emails = await restaurantDbContext.Customerdata
        .Where(c => c.UserId == userId)
        .Select(c => c.Email)
        .ToListAsync();
            if (emails == null || !emails.Any())
            {
                return NotFound("No emails found for the given user ID.");
            }

            return Ok(emails);
        }
        

    }
}
