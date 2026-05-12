namespace Termini_Api
{
    public record RabbitMqOptions
    {
        public string ConnectionString { get; init; } = string.Empty;
    }
}