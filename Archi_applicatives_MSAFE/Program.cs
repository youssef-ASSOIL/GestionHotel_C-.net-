var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable middleware for Swagger
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Optional: config => config.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();