using LogCentralizer.Backend.Repository.Worker.MongoDB.Entities;
using LogCentralizer.Backend.Worker.Providers;
using LogCentralizer.Backend.Worker.Repository.MongoDB.Repositories;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MongoDB.Driver;
using Serilog;

namespace LogCentralizer.Backend.Worker
{
    public class LogConsumer : IConsumer<FiveMLogs>
    {

        private readonly IMongoRepository<FiveMLog> _LogRepository;
        public LogConsumer()
        {
        }

        public Task Consume(ConsumeContext<FiveMLogs> context)
        {

            
            var queueName = context.ReceiveContext.InputAddress.GetReceiveSettings().QueueName;
            var citySettings = CitySettingsProvider.AllCitySettings.FirstOrDefault(c => c.QueueName == queueName);
            
            try
            {
                // var logRepository = _logRepositories[citySettings.CityName];

                Log.Information("[{CityName}] | User_id: {user_id} | Target_id: {target_id} | Room: {room} | Category: {category} | Event_time: {event_time}",
                    citySettings.CityName, context.Message.UserId, context.Message.TargetId, context.Message.Room, context.Message.Category, DateTimeOffset.FromUnixTimeSeconds(context.Message.Time).UtcDateTime);
            }
            catch (Exception ex)
            {
                throw new Exception(context.Message.ToString());

            }

            return Task.CompletedTask;
        }
    }
}

