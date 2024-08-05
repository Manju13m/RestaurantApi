using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Data;
using RestaurantAPI.excel;
using RestaurantAPI.Models;
using RestaurantAPI.Models.ViewModels;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminApiController : Controller
    {
        private readonly RestaurantDbContext restrauntDbContext;
        private readonly IExcelReportGenerator excelReportGenerator;

        public AdminApiController(RestaurantDbContext restrauntDbContext, IExcelReportGenerator excelReportGenerator)
        {
            this.restrauntDbContext = restrauntDbContext;
            this.excelReportGenerator = excelReportGenerator;
        }

        [HttpGet("dashboard-data")]
        public async Task<IActionResult> GetAdminDashboardData()
        {
            
            // Get total customers registered in the last 7 days
            var totalCustomersLast7Days = await restrauntDbContext.Customerdata
                .Where(c => c.CreatedAt >= DateTime.UtcNow.AddDays(-7))
                .CountAsync();

            // Get total bookings done for the next 3 days
            var totalBookingsNext3Days = await restrauntDbContext.Bookingdata
                .Where(b => b.BookingDate >= DateTime.UtcNow && b.BookingDate <= DateTime.UtcNow.AddDays(3))
                .CountAsync();

            // Get total cancellations in the last 3 days
            var totalCancellationsLast3Days = await restrauntDbContext.Bookingdata
                .Where(b => b.Status == BookingStatus.Cancelled && b.BookingDate >= DateTime.UtcNow.AddDays(-3))
                .CountAsync();

            // Get upcoming bookings details
            DateTime today = DateTime.Today;
            DateTime threeDaysLater = today.AddDays(3);

            var upcomingBookings = await restrauntDbContext.Bookingdata
                .Where(b => b.BookingDate >= today && b.BookingDate <= threeDaysLater && b.Status == BookingStatus.Booked)
                .Select(b => new AddBookRequest
                {
                    UserId = b.UserId,
                    BookingId = b.BookingId,
                    FromTime = b.FromTime,
                    ToTime = b.ToTime,
                    BookingDate = b.BookingDate,
                    TableNumber = b.TableNumber
                })
                .ToListAsync();

            var dashboardData = new
            {
                TotalCustomersLast7Days = totalCustomersLast7Days,
                TotalBookingsNext3Days = totalBookingsNext3Days,
                TotalCancellationsLast3Days = totalCancellationsLast3Days,
                UpcomingBookings = upcomingBookings
            };

            return Ok(dashboardData);
        }




        [HttpGet("download-bookings-report")]
        public IActionResult DownloadBookingsReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var report = excelReportGenerator.GenerateBookingsReport(startDate, endDate);
                return File(report, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BookingsReport.xlsx");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, new { Message = $"Error generating bookings report: {ex.Message}" });
            }
        }

    }
}
