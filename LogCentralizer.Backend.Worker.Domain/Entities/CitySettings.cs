namespace LogCentralizer.Backend.Worker.Domain.Entities
{
    public class CitySettings
    {
        public string CityName { get; set; } = null!;
        public string QueueName { get; set; } = null!;
        public uint MessageCount { get; set; }
        public uint ConsumerCount { get; set; }
        public RabbitMqSettings RabbitMqSettings { get; set; } = new();
        public MongoDbSettings MongoDbSettings { get; set; } = new();
    }
}
