using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Termini_Api.DTOs;
using Termini_Api.Models;
using Termini_Api.TerminiDbContext;

public class TerminConsumerService : BackgroundService
{
    private readonly IConnection _connection;
    private IChannel _channel;
    private readonly IServiceProvider _serviceProvider;

    public TerminConsumerService(IConnection  connection, IServiceProvider serviceProvider)
    {
        _connection = connection;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        
        var consumer = new  AsyncEventingBasicConsumer(_channel);
 
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var dto = JsonSerializer.Deserialize<TerminDTO>(message);

            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TerminiDBContext>();

            var teren = await db.Terens.FindAsync(dto.TerenId);
            var beneficiary = await db.Beneficiaries.FindAsync(dto.BeneficiaryId);

            if (teren == null || beneficiary == null)
            {
                // discard
                return;
            }

            bool exists = db.Termins.Any(t =>
                t.TerminOd == dto.TerminOd &&
                t.TerminDo == dto.TerminDo &&
                t.Teren.TerenId == dto.TerenId);

            if (!exists)
            {
                var durationHours = (dto.TerminDo - dto.TerminOd).TotalHours;

                var pricePerHour = await db.TerminPrices
                .Where(tp => tp.TerenId == dto.TerenId)
                .Select(tp => tp.Price)
                .FirstOrDefaultAsync();

                var fullPrice = pricePerHour * (decimal)durationHours;

                var termin = new Termin
                {
                    TerminOd = dto.TerminOd,
                    TerminDo = dto.TerminDo,
                    Teren = teren,
                    Beneficiary = beneficiary,
                    FullPrice = fullPrice,
                    IsRated = false
                };

                await db.Termins.AddAsync(termin,cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }
        };

        _channel.BasicConsumeAsync(queue: "termins",
                              autoAck: true,
                              consumer: consumer,
                              cancellationToken);
    }
}
