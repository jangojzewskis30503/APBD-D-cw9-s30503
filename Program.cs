using Microsoft.EntityFrameworkCore;
using WebApplication5.Data;
using WebApplication5.Services;

namespace WebApplication5;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        
        builder.Services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
        });
        
        // WAŻNE: Rejestracja serwisu PRZED builder.Build()
        builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();

        // Budowanie aplikacji po zarejestrowaniu wszystkich serwisów
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection(); // Dodane dla bezpieczeństwa
        app.UseRouting(); // Explicit routing
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}