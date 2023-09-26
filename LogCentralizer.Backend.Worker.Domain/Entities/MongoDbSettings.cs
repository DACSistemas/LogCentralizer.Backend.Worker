namespace LogCentralizer.Backend.Worker.Domain.Entities
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = null!; 
        public string DatabaseName { get; set; } = null!;

    }
}
