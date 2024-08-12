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


                    var emailResponse = await _httpClient.GetAsync($"api/yourcontroller/getEmail?userId={model.UserId}");
                    if (emailResponse.IsSuccessStatusCode)
                    {
                        // var emailList = JsonConvert.DeserializeObject<List<string>>(await emailResponse.Content.ReadAsStringAsync());
                        var emailList = await emailResponse.Content.ReadFromJsonAsync<List<string>>();
                        // if (emailList != null && emailList.Count > 0)
                        foreach (var email in emailList)
                        {
                            // For simplicity, assuming the first email in the list is the primary email to send to
                            //var email = emailList.First();

                            // Define the email subject and message
                            string subject = "Your Checkout Details";
                            string message = $"Dear Customer,\n\nThank you for your recent visit. Your checkout is complete.\n\nBest regards,\nTrupthi Restaurant";

                            // Send the email
                            try
                            {
                                await _emailService.SendEmailAsync(email, subject, message);
                            }
                            catch (Exception emailEx)
                            {
                                // Log email sending error
                                ModelState.AddModelError("", $"Error sending email to {email}: {emailEx.Message}");
                            }
                        }
                      /*  else
                        {
                            ModelState.AddModelError("", "No emails were found for the user.");
                        }*/

                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to retrieve the customer's email.");
                    }



                    return RedirectToAction("AdminDashboard", "Admin");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"{errorContent}");
                }


                /* if (response.IsSuccessStatusCode)
                 {
                     var email = await GetCustomerEmailAsync(model.UserId);

                     if (!string.IsNullOrEmpty(email))
                     {
                         // Validate email format
                         if (!IsValidEmail(email))
                         {
                             ModelState.AddModelError("", "The retrieved email address is invalid.");
                             return View(model);
                         }

                         string subject = "Your Checkout Details";
                         string message = $"Dear Customer,\n\nThank you for your recent visit. Your checkout is complete.\n\nBest regards,\nTrupthi Restaurant";

                         await _emailService.SendEmailAsync(email, subject, message);

                         return RedirectToAction("AdminDashboard", "Admin");
                     }
                     else
                     {
                         ModelState.AddModelError("", "Failed to retrieve email.");
                     }
                 } 
                 else
                 {
                     var errorContent = await response.Content.ReadAsStringAsync();
                     ModelState.AddModelError("", $"Checkout API error: {response.StatusCode} - {errorContent}");
                 }*/
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            }

            return View(model);
        }


        /* private bool IsValidEmail(string email)
         {
             try
             {
                 var mailAddress = new MailAddress(email);
                 return true;
             }
             catch (FormatException)
             {
                 return false;
             }
         }
        */

        /*  private async Task<string> GetCustomerEmailAsync(string userId)
          {
              try
              {
                  var emailResponse = await _httpClient.GetAsync($"api/CheckInOutApi/getEmail?userId={userId}");

                  if (emailResponse.IsSuccessStatusCode)
                  {
                      var email = await emailResponse.Content.ReadAsStringAsync();

                      // Sanitize and validate email
                      email = email.Trim();

                      // Simple validation to check if the email contains invalid characters
                      if (email.Contains("\""))
                      {
                          throw new FormatException("The email address contains invalid characters.");
                      }

                      return email;
                  }
                  else
                  {
                      // Handle error if email retrieval fails
                      var errorContent = await emailResponse.Content.ReadAsStringAsync();
                      ModelState.AddModelError("", $"Failed to retrieve email: {errorContent}");
                      return null;
                  }
              }
              catch (Exception ex)
              {
                  // Log and handle the exception
                  ModelState.AddModelError("", $"An error occurred while retrieving email: {ex.Message}");
                  return null;
              }
          }*/




    }
}

