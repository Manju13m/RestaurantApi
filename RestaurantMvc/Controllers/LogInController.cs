using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using RestaurantMvc.Models.ViewModels;
using RestaurantMvc.Models;

namespace RestaurantMvc.Controllers
{
    public class LogInController : Controller
    {
        private readonly HttpClient _httpClient;
        public LogInController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("LoginApiClient");

        }

        [HttpGet]
        public IActionResult Log()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Log(AddLogRequest addLogRequest)
        {
            if (ModelState.IsValid)
            {

                // Serialize request to JSON
                var requestContent = new StringContent(JsonConvert.SerializeObject(addLogRequest), Encoding.UTF8, "application/json");

                // Call the Web API
                var response = await _httpClient.PostAsync("api/LoginApi", requestContent);

                

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

                    // Check for null or empty values
                    if (result == null ||
                        string.IsNullOrEmpty(result.UserId) ||
                        string.IsNullOrEmpty(result.Email) ||
                        string.IsNullOrEmpty(result.Name) ||
                        string.IsNullOrEmpty(result.Role))
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login data received.");
                        return View(addLogRequest);
                    }

                    // Create claims based on role
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, result.Name),
                        new Claim(ClaimTypes.Email, result.Email),
                        new Claim(ClaimTypes.Role, result.Role)
                    };

                    // Add UserId claim only if the role is Customer
                    if (result.Role == "Customer")
                    {
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, result.UserId));
                    }

                    // Create identity object
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Sign in the user
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    // Redirect based on role
                    if (result.Role == "Customer")
                    {
                        return RedirectToAction("CustomerDashboard", "Customer");
                    }
                    else if (result.Role == "Admin")
                    {
                        return RedirectToAction("AdminDashboard", "Admin");
                    }
                }

                // Handle error
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(addLogRequest);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            // Sign out the user
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Clear the user's session
            HttpContext.Session.Clear();

            // Redirect to the login page (or any other page)
            return RedirectToAction("Log", "LogIn");
        }

    }
}
