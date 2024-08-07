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
        public IActionResult CheckIn(string bookingId, string userId)
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
                var response = await _httpClient.PostAsync("/api/CheckInOutApi/Checkin", content);

                if (response.IsSuccessStatusCode)
                {
                  
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
                
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult CheckOut(string bookingId, string userId)
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
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // Call Checkout API
                var checkoutResponse = await _httpClient.PostAsync("/api/CheckInOutApi/Checkout", content);

                if (checkoutResponse.IsSuccessStatusCode)
                {
                    // Get customer email from API
                    var emailResponse = await _httpClient.GetAsync($"/api/CheckInOutApi/getEmail?userId={model.UserId}");
                    if (emailResponse.IsSuccessStatusCode)
                    {
                        var customerEmail = await emailResponse.Content.ReadAsStringAsync();

                        // Send email to the customer using the EmailService
                        var subject = "Checkout Confirmation";
                        var message = $"Dear customer, your checkout for booking ID {model.BookingId} was successful. Thank you for dining with us!";

                        await _emailService.SendEmailAsync(customerEmail, subject, message);

                        return RedirectToAction("AdminDashboard", "Admin");
                    }
                    else
                    {
                        var errorContent = await emailResponse.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"Email error: {emailResponse.StatusCode} - {errorContent}");
                    }
                }
                else
                {
                    var errorContent = await checkoutResponse.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"API error: {checkoutResponse.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            }

            return View(model);
        }

    }
}
