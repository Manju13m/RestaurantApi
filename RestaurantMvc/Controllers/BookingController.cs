using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestaurantMvc.email;
using RestaurantMvc.Models;
using RestaurantMvc.Models.ViewModels;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace RestaurantMvc.Controllers
{
    public class BookingController : Controller
    {

        private readonly HttpClient _httpClient;
        private readonly IEmailService _emailService;
        private readonly ILogger<BookingController> _logger;

        public BookingController(IHttpClientFactory httpClientFactory, IEmailService emailService, ILogger<BookingController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("CheckOutApiClient");
            _emailService = emailService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Book()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get UserId from claims

            AddBookRequest bookingViewModel = new AddBookRequest
            {
                UserId = userId
            };

            return View(bookingViewModel);
        }

        [HttpPost]

        public async Task<IActionResult> Book(AddBookRequest addBookRequest)
        {

            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync("api/bookingapi/book", addBookRequest);

                if (response.IsSuccessStatusCode)
                {
                    var booking = await response.Content.ReadFromJsonAsync<Booking>();

                    // Retrieve admin emails from the API
                    var adminResponse = await _httpClient.GetAsync("api/bookingapi/emails");
                    if (adminResponse.IsSuccessStatusCode)
                    {
                        var adminEmails = await adminResponse.Content.ReadFromJsonAsync<List<string>>();

                        // Send booking notification email to each admin
                        foreach (var adminEmail in adminEmails)
                        {
                            string adminSubject = "New Booking at Trupthi Restaurant";
                            string adminMessage = $"Dear Admin,\n\nA new booking has been made at Trupthi Restaurant:\n\nCustomer ID: {booking.UserId}\nCustomer Name: {addBookRequest.CustomerName}\nBooking Date: {addBookRequest.BookingDate.ToShortDateString()}\nTime Slot: {addBookRequest.FromTime} to {addBookRequest.ToTime}\nTable Number: {addBookRequest.TableNumber}\n\nPlease review the details and manage accordingly.\n\nBest regards,\nTrupthi Restaurant Team";

                            await _emailService.SendEmailAsync(adminEmail, adminSubject, adminMessage);
                        }
                    }
                    else
                    {
                        _logger.LogError("Failed to retrieve admin emails.");
                    }

                    // Send booking confirmation email to customer
                    var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                    if (userEmail != null)
                    {
                        string subject = "Booking Confirmation at Trupthi Restaurant";
                        string message = $"Dear {addBookRequest.CustomerName},\n\nWe are pleased to confirm your booking at Trupthi Restaurant.\n\nHere are the details of your reservation:\nBooking Date: {addBookRequest.BookingDate.ToShortDateString()}\nTime Slot: {addBookRequest.FromTime} to {addBookRequest.ToTime}\nTable Number: {addBookRequest.TableNumber}.\n\nThank you for choosing Trupthi Restaurant. We look forward to serving you.\n\nBest regards,\nTrupthi Restaurant Team";
                        await _emailService.SendEmailAsync(userEmail, subject, message);
                    }

                    return RedirectToAction("CustomerDashboard", "Customer");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    ViewBag.ConflictMessage = await response.Content.ReadAsStringAsync();
                    return View(addBookRequest);
                }
                else
                {
                    ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                    return View(addBookRequest);
                }
            }
            return View(addBookRequest);

        }

        [HttpPost]
        public async Task<IActionResult> Cancel(Guid bookingId)
        {
            var response = await _httpClient.PostAsync($"api/bookingapi/cancel/{bookingId}", null);

            if (response.IsSuccessStatusCode)
            {
                // Retrieve the booking details from the API response
                var bookingDetails = await response.Content.ReadFromJsonAsync<AddBookRequest>(); // Assuming this is the correct type

                // Send cancellation email to the customer
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                if (userEmail != null)
                {
                    string subject = "Booking Cancellation Confirmation at Trupthi Restaurant";
                    string message = $"Dear {bookingDetails.CustomerName},\n\nWe have received your request to cancel your booking at Trupthi Restaurant on {bookingDetails.BookingDate.ToShortDateString()} from {bookingDetails.FromTime} to {bookingDetails.ToTime} for table number {bookingDetails.TableNumber}.\n\nyour booking has been succesfully cancelled\n\n Thank you for choosing Trupthi Restaurant.We hope to welcome you again soon!\n\nBest regards,\nTrupthi Restaurant Team";
                    await _emailService.SendEmailAsync(userEmail, subject, message);
                }

                // Retrieve admin emails from the API
                var adminResponse = await _httpClient.GetAsync("api/bookingapi/emails");
                if (adminResponse.IsSuccessStatusCode)
                {
                    var adminEmails = await adminResponse.Content.ReadFromJsonAsync<List<string>>();

                    // Send booking cancellation notification email to each admin
                    foreach (var adminEmail in adminEmails)
                    {
                        string adminSubject = "Booking Cancellation at Trupthi Restaurant";
                        string adminMessage = $"Dear Admin,\n\nA booking has been cancelled at Trupthi Restaurant:\n\nCustomer Name: {bookingDetails.CustomerName}\nBooking Date: {bookingDetails.BookingDate.ToShortDateString()}\n Time Slot: {bookingDetails.FromTime} to {bookingDetails.ToTime}\nTable Number: {bookingDetails.TableNumber}\n\nPlease review the details and manage accordingly.\n\nBest regards,\nTrupthi Restaurant Team";
                        await _emailService.SendEmailAsync(adminEmail, adminSubject, adminMessage);
                    }
                }

                // Redirect to Customer Dashboard
                return RedirectToAction("CustomerDashboard", "Customer");
            }
            else
            {
                ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                return View("Error"); // Handle the error appropriately
            }
        }



    }

}
