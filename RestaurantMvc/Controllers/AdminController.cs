using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestaurantMvc.Models.ViewModels;

namespace RestaurantMvc.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;
        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("AdminApiClient");

        }
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        [Authorize]
        public async Task<IActionResult> AdminDashboard()
        {
            var dashboardData = await GetDashboardDataAsync();
            var adminName = User.Identity.Name; // Get the logged-in admin's name
            dashboardData.AdminName = adminName;

            return View(dashboardData);
        }

        private async Task<AdminDashboardData> GetDashboardDataAsync()
        {
            var response = await _httpClient.GetStringAsync("api/AdminApi/dashboard-data");
            return JsonConvert.DeserializeObject<AdminDashboardData>(response);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadBookingsReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Log or check the values of startDate and endDate
                Console.WriteLine($"Start Date: {startDate}, End Date: {endDate}");

                var response = await _httpClient.GetAsync($"api/adminapi/download-bookings-report?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

                if (response.IsSuccessStatusCode)
                {
                    var report = await response.Content.ReadAsByteArrayAsync();
                    return File(report, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BookingsReport.xlsx");
                }
                else
                {
                    ViewBag.ErrorMessage = "Error generating bookings report.";
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                return View("Error");
            }
        }




    }
}
