using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models;
using RestaurantAPI.Models.ViewModels;

namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingApiController : Controller
    {
        private readonly RestaurantDbContext restaurantDbContext;

        public BookingApiController(RestaurantDbContext restaurantDbContext)
        {
            this.restaurantDbContext = restaurantDbContext;
        }

       
            [HttpPost("book")]
            public async Task<IActionResult> Book([FromBody] AddBookRequest addBookRequest)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var isConflict = await restaurantDbContext.Bookingdata.AnyAsync(b => b.TableNumber == addBookRequest.TableNumber &&
                                                                             b.BookingDate == addBookRequest.BookingDate &&
                                                                             ((addBookRequest.FromTime >= b.FromTime && addBookRequest.FromTime < b.ToTime) ||
                                                                              (addBookRequest.ToTime > b.FromTime && addBookRequest.ToTime <= b.ToTime) ||
                                                                              (addBookRequest.FromTime <= b.FromTime && addBookRequest.ToTime >= b.ToTime)));
                if (isConflict)
                    return Conflict("The table is not available for the selected time slot.");

                var booking = new Booking
                {
                    BookingId = Guid.NewGuid(),
                    UserId = addBookRequest.UserId,
                    CustomerName = addBookRequest.CustomerName,
                    BookingDate = addBookRequest.BookingDate,
                    FromTime = addBookRequest.FromTime,
                    ToTime = addBookRequest.ToTime,
                    TableNumber = addBookRequest.TableNumber
                };

                try
                {
                restaurantDbContext.Bookingdata.Add(booking);
                    await restaurantDbContext.SaveChangesAsync();
                    return Ok(booking);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Internal server error.");
                }
            }

        [HttpPost("cancel/{bookingId}")]
        public async Task<IActionResult> Cancel(Guid bookingId)
        {
            var booking = await restaurantDbContext.Bookingdata
                .Include(b => b.Customer)  // Assuming you have a Customer relationship
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
                return NotFound();

            if (booking.BookingDate > DateTime.Now.AddHours(24))
            {
                booking.Status = BookingStatus.Cancelled;

                try
                {
                    restaurantDbContext.Update(booking);
                    await restaurantDbContext.SaveChangesAsync();

                    // Return the booking details needed for email notification
                    var bookingDetails = new
                    {
                        booking.Customer.Email,
                        booking.CustomerName,
                        booking.BookingDate,
                        booking.FromTime,
                        booking.ToTime,
                        booking.TableNumber
                    };

                    return Ok(bookingDetails);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Internal server error.");
                }
            }
            else
            {
                return BadRequest("Booking cannot be cancelled less than 24 hours before the booking time.");
            }
        }


        [HttpGet("emails")]
        public async Task<IActionResult> GetAdminEmails()
        {
            var emails = await restaurantDbContext.Admindata
                .Select(a => a.Email)
                .ToListAsync();

            return Ok(emails);
        }

    }
}
