namespace LogCentralizer.Backend.Worker.Domain.Entities
{
    public record RabbitMqSettings
    {
        public string Host { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
