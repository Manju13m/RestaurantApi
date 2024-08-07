using Microsoft.AspNetCore.Mvc;

using RestaurantMvc.email;
using RestaurantMvc.Models.ViewModels;
using System.Text;
using System.Text.Json;


namespace RestaurantMvc.Controllers
{
    public class CheckInOutController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IEmailService _emailService;

        public CheckInOutController(IHttpClientFactory httpClientFactory, IEmailService emailService)
        {
            _httpClient = httpClientFactory.CreateClient("CheckInOutApiClient");
            _emailService = emailService;
        }


        [HttpGet]
        public IActionResult CheckIn(Guid bookingId, string userId)
        {
            var model = new CheckInOutViewModel
            {
                BookingId = bookingId,
                UserId = userId
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> CheckIn(CheckInOutViewModel model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("api/CheckInOutApi/Checkin", content);

                if (response.IsSuccessStatusCode)
                {
                    // Optionally send an email notification
                    // await _emailService.SendCheckInConfirmationEmail(model.UserId, model.BookingId);

                    return RedirectToAction("AdminDashboard", "Admin");
                }
                else
                {
                    // Log and handle the error response
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"API error: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                // Log the exception and show a friendly error message
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult CheckOut(Guid bookingId, string userId)
        {
            var model = new CheckInOutViewModel
            {
                BookingId = bookingId,
                UserId = userId
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CheckOut(CheckInOutViewModel model)
        {
            try
            {
                // Serialize model to JSON
                var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

                // Post checkout data
                var response = await _httpClient.PostAsync("/api/CheckInOutApi/Checkout", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    // Retrieve the customer's email
                    var emailResponse = await _httpClient.GetAsync($"/api/CheckInOutApi/{model.UserId}");

                    if (emailResponse.IsSuccessStatusCode)
                    {
                        var email = await emailResponse.Content.ReadAsStringAsync();
                       // email = email.Trim();

                        // Compose subject and body
                        string subject = "Your Checkout Details";
                        string message = $"Dear Customer,\n\nThank you for dining with us! \n\nYour gross amount for the recent visit is ₹{model.GrossAmount:N0}. \n\nWe appreciate your patronage. \n\nBest regards, \nTrupthi Restaurant";

                        // Send email
                        await _emailService.SendEmailAsync(email, subject, message);

                        return RedirectToAction("AdminDashboard", "Admin");
                    }
                    else
                    {
                        // Handle error if email retrieval fails
                        var errorContent = await emailResponse.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"Failed to retrieve email: {errorContent}");
                    }
                }
                else
                {
                    // Handle error response
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"API error: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (implement logging here)
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            }

            return View(model);
        }
    }
}
