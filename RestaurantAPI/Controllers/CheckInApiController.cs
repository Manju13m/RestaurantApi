using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Data;
using RestaurantAPI.Models;
using RestaurantAPI.Models.ViewModels;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckInApiController : Controller
    {
        private readonly RestaurantDbContext restaurantDbContext;

        public CheckInApiController(RestaurantDbContext restaurantDbContext)
        {
            this.restaurantDbContext = restaurantDbContext;
        }

        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var checkIn = new CheckIn
                {
                    CustomerId = model.CustomerId,
                    CheckInDate = model.CheckInDate,
                    CheckInTime = model.CheckInTime
                };

                restaurantDbContext.CheckIns.Add(checkIn);
                await restaurantDbContext.SaveChangesAsync();

                return Ok(new { message = "Check-in successful." });
            }

            return BadRequest(ModelState);
        }
    }
}
