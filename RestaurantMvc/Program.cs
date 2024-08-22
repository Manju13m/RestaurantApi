using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Net.Http.Headers;
using OfficeOpenXml;
using RestaurantMvc;
using RestaurantMvc.email;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add email service
builder.Services.AddScoped<IEmailService, EmailService>();

// Set EPPlus LicenseContext to NonCommercial
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;



//Configure HttpClient
builder.Services.AddHttpClient("RegisterApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:44390");
      
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});



builder.Services.AddHttpClient("LoginApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:44390");

    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});


builder.Services.AddHttpClient("AdminApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:44390");

    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});





builder.Services.AddHttpClient("CustomerApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:44390");

    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient("BookingApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:44390");

    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});


builder.Services.AddHttpClient("CheckInOutApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:44390");

    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});





// Add IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Configure cookie-based authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Log"; // Set the login path
        options.AccessDeniedPath = "/Login/AccessDenied"; // Set the access denied path
    });

// Add session services
builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the session timeout
    options.Cookie.HttpOnly = true; // Make the session cookie HTTP-only
    options.Cookie.IsEssential = true; // Make the session cookie essential
});



var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Response.Headers[HeaderNames.CacheControl] = "no-store, no-cache, must-revalidate";
    context.Response.Headers[HeaderNames.Pragma] = "no-cache";
    context.Response.Headers[HeaderNames.Expires] = "0";
    await next();
});




// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable authentication and authorization
app.UseAuthentication();

app.UseAuthorization();

// Enable session middleware
app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
