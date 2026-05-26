using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;
using Termini_Api;
using Termini_Api.TerminiDbContext;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<TerminiDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DBConnection")));

// RabbitMQ connection
// Build the RabbitMQ connection once at startup
var factory = new ConnectionFactory
{
    Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ"))
};

var connection = await factory.CreateConnectionAsync();
builder.Services.AddSingleton<IConnection>(connection);

// Register a channel factory using async API
builder.Services.AddSingleton<Func<Task<IChannel>>>(sp =>
{
    var conn = sp.GetRequiredService<IConnection>();
    return async () =>
    {
        var channel = await conn.CreateChannelAsync();
        await channel.QueueDeclareAsync(
            queue: "termins",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        return channel;
    };
});
// Background service za consumer
builder.Services.AddHostedService<TerminConsumerService>();

// JWT config
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
