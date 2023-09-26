namespace LogCentralizer.BackEnd.Worker.Repository.MongoDB.Settings
{
    public class MongoDbSettings : IMongoDbSettings
    {
        public const string KeyName = "MongoDbSettings";
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

    }
}
