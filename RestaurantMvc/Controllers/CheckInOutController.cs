using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestaurantMvc.email;
using RestaurantMvc.Models.ViewModels;
using System.Net.Mail;
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
            // var json = JsonSerializer.Serialize(model);
            // var content = new StringContent(json, Encoding.UTF8, "application/json");

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

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
                    ModelState.AddModelError("", $"{errorContent}");
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
            try
            {
                var requestContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/CheckInOutApi/Checkout", requestContent);

                if (response.IsSuccessStatusCode && response.Content != null)
                {

                    return RedirectToAction("AdminDashboard", "Admin");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"{errorContent}");
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

