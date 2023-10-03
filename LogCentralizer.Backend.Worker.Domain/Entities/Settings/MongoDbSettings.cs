namespace LogCentralizer.BackEnd.Worker.Repository.Settings
{
    public class MongoDbSettings : IMongoDbSettings
    {
        public const string KeyName = "MongoDbSettings";
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

    }
}
