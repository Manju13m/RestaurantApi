using Microsoft.AspNetCore.Mvc;
using RestaurantMvc.Models.ViewModels;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestaurantMvc.Controllers
{
    public class CheckInController : Controller
    {
        private readonly HttpClient _httpClient;

        public CheckInController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CheckInApiClient");
        }

        [HttpGet]
        public IActionResult CheckIn()
        {
            return View(new CheckInViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CheckIn(CheckInViewModel model)
        {
            if (ModelState.IsValid)
            {
                

                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/CheckInApi/check-in", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("CheckInConfirmation");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error occurred during check-in.");
                }
            }

            return View(model);
        }

        public IActionResult CheckInConfirmation()
        {
            return View();
        }
    }
}

