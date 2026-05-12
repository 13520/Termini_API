using System;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Termini_Api;
using Termini_Api.TerminiDbContext;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<TerminiDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DBConnection")));

// --- RabbitMQ: read from configuration and register as options ---
// Supports these locations (in order of precedence):
// 1) "RabbitMQ" (plain string containing amqp://...)
// 2) "RabbitMQ:ConnectionString" (section with ConnectionString property)
// 3) ConnectionStrings:RabbitMQ

// Bind section (if present)
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMQ"));

// Explicit fallback / override when connection string is provided as a single value
var rabbitConn = builder.Configuration["RabbitMQ"]
                 ?? builder.Configuration["RabbitMQ:ConnectionString"]
                 ?? builder.Configuration.GetConnectionString("RabbitMQ");

if (!string.IsNullOrWhiteSpace(rabbitConn))
{
    // Override or set the ConnectionString value
    builder.Services.Configure<RabbitMqOptions>(opts => 
        new RabbitMqOptions { ConnectionString = rabbitConn });
}
// ---------------------------------------------------------------

var app = builder.Build();

// Log the (partially masked) RabbitMQ connection string at startup for verification
try
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    var rabbitOptions = app.Services.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
    if (!string.IsNullOrWhiteSpace(rabbitOptions.ConnectionString))
    {
        var shown = rabbitOptions.ConnectionString.Length > 20
            ? rabbitOptions.ConnectionString.Substring(0, 20) + "..."
            : rabbitOptions.ConnectionString;
        logger.LogInformation("RabbitMQ connection string configured: {ConnPreview}", shown);
    }
    else
    {
        logger.LogInformation("RabbitMQ connection string not configured.");
    }
}
catch
{
    // ignore logging failures during startup
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
