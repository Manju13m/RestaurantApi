using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.Models.ViewModels;
using RestaurantAPI.password;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginApiController : Controller
    {
        private readonly RestaurantDbContext restaurantDbContext;
        private readonly PasswordService passwordService;

        public LoginApiController(RestaurantDbContext restaurantDbContext, PasswordService passwordService)
        {
            this.restaurantDbContext = restaurantDbContext;
            this.passwordService = passwordService;
        }



        [HttpPost]
        public async Task<IActionResult> Login([FromBody] AddLogRequest addLogRequest)
        {
            if (addLogRequest == null)
                return BadRequest("Invalid login request.");

            // Check if UserId belongs to a Customer
            var customer = await restaurantDbContext.Customerdata.FirstOrDefaultAsync(c => c.UserId == addLogRequest.UserId);

            if (customer != null && passwordService.VerifyPassword(addLogRequest.Password, customer.PasswordHash, customer.PasswordSalt))
            {
                return Ok(new
                {
                    UserId = customer.UserId,
                    Email = customer.Email,
                    Name = $"{customer.FirstName} {customer.LastName}",
                    Role = "Customer"
                });
            }

            // Check if UserId belongs to an Admin
            var admin = await restaurantDbContext.Admindata
                 .FirstOrDefaultAsync(a => a.UserId == addLogRequest.UserId);

            if (admin != null && passwordService.VerifyPassword(addLogRequest.Password, admin.PasswordHash, admin.PasswordSalt))
            {
                return Ok(new
                {
                    UserId = admin.UserId,
                    Email = admin.Email,
                    Name = $"{admin.FirstName} {admin.LastName}",
                    Role = "Admin"
                });
            }

            return Unauthorized("Invalid login attempt.");
        }
    }
}
