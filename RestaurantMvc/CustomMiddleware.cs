
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace RestaurantMvc
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // List of protected paths where the user should be logged out if they navigate away
            var protectedPaths = new[] { "/Customer/CustomerDashboard", "/Admin/AdminDashboard" };

            // Check if the user is navigating away from a protected page
            if (context.User.Identity.IsAuthenticated && !protectedPaths.Any(p => context.Request.Path.StartsWithSegments(p)))
            {
                // Log the user out
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                context.Session.Clear();
            }

            await _next(context);
        }
    }
}
