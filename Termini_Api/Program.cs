using System;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Termini_Api.TerminiDbContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<TerminiDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DBConnection")));

var app = builder.Build();

// Apply EF Core migrations on startup with retry loop
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var db = services.GetRequiredService<TerminiDBContext>();

    const int maxAttempts = 20;
    var attempt = 0;
    var delay = TimeSpan.FromSeconds(3);

    while (true)
    {
        try
        {
            attempt++;
            logger.LogInformation("Attempting database migrate (attempt {Attempt}/{Max}).", attempt, maxAttempts);
            db.Database.Migrate();
            logger.LogInformation("Database migration applied successfully.");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Database migrate attempt {Attempt} failed.", attempt);

            if (attempt >= maxAttempts)
            {
                logger.LogError("Exceeded maximum migration attempts ({Max}). Migrations were NOT applied.", maxAttempts);
                break;
            }

            Thread.Sleep(delay);
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
