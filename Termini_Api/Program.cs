using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using System.Text;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Termini_Api.TerminiDbContext;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        // Add Bearer token security definition
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = "Enter 'Bearer {your JWT token}'"
        };
        
        // // Apply Bearer requirement globally
        
        document.Security ??= new List<OpenApiSecurityRequirement>();
        var securityRequirement = new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>() 
        };
        document.Security.Add(securityRequirement);
        
        return Task.CompletedTask;
    });
});


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
builder.Services.AddSingleton<IChannel>(sp =>
{
    var conn = sp.GetRequiredService<IConnection>();
    var channel = conn.CreateChannelAsync().GetAwaiter().GetResult(); // blokira async
    channel.QueueDeclareAsync(
        queue: "termins",
        durable: true,
        exclusive: false,
        autoDelete: false,
        arguments: null
    ).GetAwaiter().GetResult();

    return channel;
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
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Termini API")
            .WithTheme(ScalarTheme.DeepSpace)
            .WithSidebar(true)
            .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Axios);
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
