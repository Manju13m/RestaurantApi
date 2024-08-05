using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestaurantMvc.email;
using RestaurantMvc.Models.ViewModels;
using System.Net.Http;
using System.Text;

namespace RestaurantMvc.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IEmailService _emailService;

        public CheckOutController(IHttpClientFactory httpClientFactory, IEmailService emailService)
        {
            _httpClient = httpClientFactory.CreateClient("CheckOutApiClient");
            _emailService = emailService;
        }
        public IActionResult CheckOut()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(CheckOutViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Call the API to create a new CheckOut record
                var checkOut = new
                {
                    CustomerId = model.CustomerId,
                    GrossAmount = model.GrossAmount
                };

                var content = new StringContent(JsonConvert.SerializeObject(checkOut), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/CheckOutApi", content);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Error creating checkout record.");
                    return View(model);
                }

                // Fetch customer email by customer ID
                var emailResponse = await _httpClient.GetAsync($"api/CheckOutApi/{model.CustomerId}");
                if (!emailResponse.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Customer email address not found.");
                    return View(model);
                }

                var email = await emailResponse.Content.ReadAsStringAsync();

                string subject = "Your Checkout Details";
                string message = $"Dear Customer,\n\nThank you for dining with us! \n\nYour gross amount for the recent visit is ₹{model.GrossAmount:N0}. \n\nWe appreciate your patronage. \n\nBest regards, \nTrupthi Restaurant";

                try
                {
                    await _emailService.SendEmailAsync(email, subject, message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error sending email: {ex.Message}");
                    return View(model);
                }

                return RedirectToAction("CheckOutConfirmation"); // Redirect to a success page or action
            }

            // If ModelState is not valid, return to the view with validation errors
            return View(model);
        }



        public IActionResult CheckOutConfirmation()
        {
            return View();
        }
    }
}