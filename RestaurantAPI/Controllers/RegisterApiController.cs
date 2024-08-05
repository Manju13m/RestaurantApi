using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Data;
using RestaurantAPI.Models;
using RestaurantAPI.password;
using RestaurantAPI.Models.ViewModels;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterApiController : Controller
    {
        private readonly RestaurantDbContext restrauntDbContext;
        
        private readonly PasswordService _passwordService;


        public RegisterApiController(RestaurantDbContext restrauntDbContext,  PasswordService passwordService)
        {
            this.restrauntDbContext = restrauntDbContext;
            
            this._passwordService = passwordService;

        }

        
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] AddRegRequest addRegRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the email is already registered
            bool emailExists = restrauntDbContext.Customerdata.Any(c => c.Email == addRegRequest.Email) ||
                               restrauntDbContext.Admindata.Any(a => a.Email == addRegRequest.Email);

            if (emailExists)
            {
                return BadRequest("This email ID is already registered. Please login using your credentials.");
            }

            // Check if the user ID is already registered
            bool userIdExists = restrauntDbContext.Customerdata.Any(c => c.UserId == addRegRequest.UserId) ||
                                restrauntDbContext.Admindata.Any(a => a.UserId == addRegRequest.UserId);

            if (userIdExists)
            {
                return BadRequest("This user ID is already registered. Please use a different user ID.");
            }

            // Hash password and generate salt
            byte[] salt = _passwordService.GenerateSalt();
            byte[] hashedPassword = _passwordService.HashPassword(addRegRequest.Password, salt);

            if (addRegRequest.Role == "Customer")
            {
                var customer = new Customer
                {
                    UserId = addRegRequest.UserId,
                    FirstName = addRegRequest.FirstName,
                    LastName = addRegRequest.LastName,
                    Address = addRegRequest.Address,
                    PasswordHash = hashedPassword,
                    PhoneNo = addRegRequest.PhoneNo,
                    Email = addRegRequest.Email,
                    PasswordSalt = salt
                };
                restrauntDbContext.Customerdata.Add(customer);
                await restrauntDbContext.SaveChangesAsync();

                

            }
            else if (addRegRequest.Role == "Admin")
            {
                var admin = new Admin
                {
                    UserId = addRegRequest.UserId,
                    FirstName = addRegRequest.FirstName,
                    LastName = addRegRequest.LastName,
                    Address = addRegRequest.Address,
                    PhoneNo = addRegRequest.PhoneNo,
                    Email = addRegRequest.Email,
                    PasswordHash = hashedPassword,
                    PasswordSalt = salt
                };
                restrauntDbContext.Admindata.Add(admin);
                await restrauntDbContext.SaveChangesAsync();

                
            }

            return Ok("Registration successful!");
        }

    }
}