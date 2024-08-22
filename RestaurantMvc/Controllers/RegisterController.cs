using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using RestaurantMvc.Models.ViewModels;
using RestaurantMvc.email;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace RestaurantMvc.Controllers
{
    public class RegisterController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IEmailService _emailService;

        public RegisterController(IHttpClientFactory httpClientFactory, IEmailService emailService)
        {
            _httpClient = httpClientFactory.CreateClient("RegisterApiClient");
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> Reg()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Clear();
            }
            return View();
        }
    

        [HttpPost]
        [ActionName("Reg")]
        public async Task<IActionResult> Reg(AddRegRequest addRegRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(addRegRequest);
            }

            // Serialize request to JSON
            var requestContent = new StringContent(JsonConvert.SerializeObject(addRegRequest), Encoding.UTF8, "application/json");

            // Call the Web API
            var response = await _httpClient.PostAsync("api/RegisterApi", requestContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Registration successful!";

                // Send a confirmation email
                if (addRegRequest.Role == "Customer")
                {
                    var subject = "Welcome to Trupthi Restaurant!";
                    var message = $"Dear {addRegRequest.FirstName},\n\nThank you for registering with Trupthi Restaurant! We are thrilled to have you as a part of our community.\n\nYour user ID is: {addRegRequest.UserId}\n\nYou can now log in to your account using this user ID and the password you created during registration.Once logged in, you can book tables,view upcoming reservations, and mange your bookings with ease\n\nWe look forward to serving you and hope you have a delightful dining experiece with us.\n\nBest regards,\nThe Trupthi Restaurant Team";
                    await _emailService.SendEmailAsync(addRegRequest.Email, subject, message);
                }
                else if (addRegRequest.Role == "Admin")
                {
                    var subject = "Welcome to Trupthi Restaurant Team!";
                    var message = $"Dear {addRegRequest.FirstName},\n\nThank you for registering with Trupthi Restaurant!\n\nYour admin user ID is: {addRegRequest.UserId}\n\nYou can now log in to your admin account using this user ID and the password you created during registration.As an admin, you will have access to mange bookings,view customer registrations, and oversee all activities within our restaurant's online system.\n\nWe are excited to have you on board and look forward to working with you to deliver exceptional service to our customers.\n\nBest regards,\nThe Trupthi Restaurant Team";
                    await _emailService.SendEmailAsync(addRegRequest.Email, subject, message);
                }

                return RedirectToAction("Reg");
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, errorMessage);
            return View(addRegRequest);
        }
    }
}