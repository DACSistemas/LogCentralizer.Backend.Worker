namespace LogCentralizer.Backend.Worker.Domain.Entities
{
    public class CitySettings
    {
        public const string KeyName = "CitySettings";
        public string CityName { get; set; } = null!;
        public string QueueName { get; set; } = null!;
        public string RabbitMqDomain { get; set; } = null!;
        public int ActiveWorkerInterval { get; set; }
        public uint MessageCount { get; set; }
        public uint ConsumerCount { get; set; }
        public MongoDbSettings MongoDbSettings { get; set; } = new();
    }
}
