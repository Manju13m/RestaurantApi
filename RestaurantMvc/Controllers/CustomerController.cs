using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestaurantMvc.Models.ViewModels;
using System.Net.Http;
using System.Security.Claims;

namespace RestaurantMvc.Controllers
{
    public class CustomerController : Controller
    {
        private readonly HttpClient _httpClient;

        public CustomerController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CustomerApiClient");
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        [Authorize]
        public async Task<IActionResult> CustomerDashboard()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Log", "Login");
            }

            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null || string.IsNullOrEmpty(claim.Value))
            {
                return RedirectToAction("Log", "Login");
            }

            string customerId = claim.Value;

            var bookings = await GetDashboardDataAsync(customerId);
            var customerName = User.Identity.Name;

            var viewModel = new CustomerDashboardViewModel
            {
                CustomerName = customerName,
                Bookings = bookings
            };

            return View(viewModel);
        }

        private async Task<List<AddBookRequest>> GetDashboardDataAsync(string customerId)
        {
            var response = await _httpClient.GetAsync($"api/CustomerApi/customer-dashboard-data?userId={customerId}");
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<AddBookRequest>>(responseContent);
        }
    }

}
           
    
   
 

