using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RestaurantAPI.Data;
using RestaurantAPI.excel;
using RestaurantAPI.password;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Inject dbcontext into application
builder.Services.AddDbContext<RestaurantDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("RestrauntDbConnectionString"))
);


// Register PasswordService as scoped service
builder.Services.AddScoped<PasswordService>();


//Add ExcelReportGenerator service
builder.Services.AddTransient<IExcelReportGenerator, ExcelReportGenerator>();

// Set EPPlus LicenseContext to NonCommercial
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
