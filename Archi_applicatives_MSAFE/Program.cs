using Archi_applicatives_MSAFE.Data;
using Archi_applicatives_MSAFE.msafe.com.business;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ➕ Add your services and DbContext
builder.Services.AddDbContext<HotelDbContext>(options =>
    options.UseMySql("server=localhost;port=3306;database=hotel_db;user=root;password=;",
        ServerVersion.AutoDetect("server=localhost;port=3306;database=hotel_db;user=root;password=;")));
builder.Services.AddScoped<ClientsServicesInterface, ClientsServicesImplementation>();
builder.Services.AddScoped<ReceptionisteServicesInterface, ReceptionisteServicesImplementation>();
builder.Services.AddScoped<PersonnelleMenageServicesInterface, PersonnelleMenageServicesImplementation>();



var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();