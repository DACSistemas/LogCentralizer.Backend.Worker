namespace LogCentralizer.Backend.Repository.Worker.MongoDB.Entities
{
    public class Payload
    {
            public string message { get; set; }
            public string category { get; set; }
            public string room { get; set; }    
            public object user_id { get; set; }
            public object target_id { get; set; }
            public long time { get; set; }
    }
}
