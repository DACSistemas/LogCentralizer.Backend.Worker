using LogCentralizer.Backend.Repository.Worker.MongoDB.Entities;
using LogCentralizer.Backend.Worker.Providers;
using LogCentralizer.Backend.Worker.Repository.MongoDB.Repositories;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Serilog;

namespace LogCentralizer.Backend.Worker
{
    public class LogConsumer : IConsumer<FiveMLogs>
    {
        private readonly IMongoRepository<FiveMLog> _logRepository;

        public LogConsumer(IMongoRepository<FiveMLog> logRepository)
        {
             _logRepository = logRepository;
        }

        public async Task Consume(ConsumeContext<FiveMLogs> context)
        {
            var queueName = context.ReceiveContext.InputAddress.GetReceiveSettings().QueueName;
            var citySettings = CitySettingsProvider.AllCitySettings[queueName];
            
            try
            {
                var logRepository = _logRepository.GetCollection(queueName);

                if (context.Message.UserId != null && int.TryParse(context.Message.UserId.ToString(), out int userIdInt) &&
                    context.Message.TargetId != null && int.TryParse(context.Message.TargetId.ToString(), out int targetIdInt))
                {
                    await logRepository.InsertOneAsync(new FiveMLog
                    {
                        UserId = userIdInt,
                        Payload = context.Message.Message,
                        Room = context.Message.Room,
                        TargetId = targetIdInt,
                        Category = context.Message.Category,
                        EventTime = DateTimeOffset.FromUnixTimeSeconds(context.Message.Time).UtcDateTime,
                    });
                }

                Log.Information("[{CityName}] | User_id: {user_id} | Target_id: {target_id} | Room: {room} | Category: {category} | Event_time: {event_time}",
                    citySettings.CityName, context.Message.UserId, context.Message.TargetId, context.Message.Room, context.Message.Category, DateTimeOffset.FromUnixTimeSeconds(context.Message.Time).UtcDateTime);
            }
            catch (Exception)
            {
                throw new Exception(context.Message.ToString());

            }

            await Task.CompletedTask;
        }
    }
}

