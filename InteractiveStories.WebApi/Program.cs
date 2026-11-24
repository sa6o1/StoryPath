using InteractiveStories.Core.Services;
using InteractiveStories.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Set ConnectionStrings:DefaultConnection using User Secrets.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<GenreService>();

// Разрешава заявки от отделния локален клиент.
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalClient", policy => policy
        .WithOrigins("http://localhost:5500", "http://127.0.0.1:5500")
        .WithMethods("GET")
        .AllowAnyHeader());
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors("LocalClient");
}
else
{
    app.UseHttpsRedirection();
}
app.UseAuthorization();
app.MapControllers();
app.Run();
