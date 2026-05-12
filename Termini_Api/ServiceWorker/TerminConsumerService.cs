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
    private readonly IModel _channel;
    private readonly IServiceProvider _serviceProvider;

    public TerminConsumerService(IModel channel, IServiceProvider serviceProvider)
    {
        _channel = channel;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
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
                var termin = new Termin
                {
                    TerminOd = dto.TerminOd,
                    TerminDo = dto.TerminDo,
                    Teren = teren,
                    Beneficiary = beneficiary
                };

                await db.Termins.AddAsync(termin);
                await db.SaveChangesAsync();
            }
        };

        _channel.BasicConsume(queue: "termins",
                              autoAck: true,
                              consumer: consumer);

        return Task.CompletedTask;
    }
}
