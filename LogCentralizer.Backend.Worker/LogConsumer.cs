using LogCentralizer.Backend.Repository.Worker.MongoDB.Entities;
using LogCentralizer.Backend.Worker.Domain.Entities;
using LogCentralizer.Backend.Worker.Providers;
using LogCentralizer.Backend.Worker.Repository.MongoDB.Repositories;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;
using System.Text.RegularExpressions;

namespace LogCentralizer.Backend.Worker
{
    public class LogConsumer : IConsumer<FiveMLogs>
    {
        private readonly IMongoRepository<FiveMLog> _logRepository;
        private readonly IMongoRepository<CategoryRole> _categoryRoleRepository;
        private readonly IMongoRepository<RoomRole> _roomRoleRepository;

        public LogConsumer(IMongoRepository<FiveMLog> logRepository, 
            IMongoRepository<CategoryRole> categoryRole, IMongoRepository<RoomRole> roomRoleRepository)
        {
             _logRepository = logRepository;
            _categoryRoleRepository = categoryRole;
            _roomRoleRepository = roomRoleRepository;
        }

        public async Task Consume(ConsumeContext<FiveMLogs> context)
        {
            var queueName = context.ReceiveContext.InputAddress.GetReceiveSettings().QueueName;
            var citySettings = CitySettingsProvider.AllCitySettings[queueName];
            
            try
            {
                var logRepository = _logRepository.GetCollection(queueName);
                var categoryRoleRepository = _categoryRoleRepository.GetCollection(queueName);
                var roomRoleRepository = _roomRoleRepository.GetCollection(queueName);

                if(!string.IsNullOrEmpty(context.Message.Category))
                {
                    var builder = Builders<CategoryRole>.Filter;
                    var mongoFilter = FilterDefinition<CategoryRole>.Empty;
                    var queryExpression = new BsonRegularExpression(new Regex(context.Message.Category, RegexOptions.IgnoreCase));
                    mongoFilter &= builder.Regex("category", queryExpression);

                    var haveRegistredCategory = await categoryRoleRepository.Find(mongoFilter).FirstOrDefaultAsync();
                    if(haveRegistredCategory is null)
                    {
                        try
                        {
                            await categoryRoleRepository.InsertOneAsync(new CategoryRole
                            {
                                Category = context.Message.Category,
                                CategoryName = context.Message.Category,
                                MinimalRole = null,
                                Icon = null,
                            });
                        }
                        catch { }

                    }
                }

                if (!string.IsNullOrEmpty(context.Message.Room) && !string.IsNullOrEmpty(context.Message.Category))
                {
                    var builderRoomRole = Builders<RoomRole>.Filter;

                    // Crie um filtro para verificar se já existe um registro com a mesma combinação de Room e Category
                    var filter = builderRoomRole.Eq("Room", context.Message.Room) &
                                 builderRoomRole.Eq("Category", context.Message.Category);

                    var existingRoomRole = await roomRoleRepository.Find(filter).FirstOrDefaultAsync();

                    if (existingRoomRole == null)
                    {
                        try
                        {
                            await roomRoleRepository.InsertOneAsync(new RoomRole
                            {
                                Room = context.Message.Room,
                                Category = context.Message.Category,
                                MinimalRole = null,
                            });
                        }
                        catch { }
                    }
                }



                int? userIdInt = null;
                int? targetIdInt = null;

                if (!string.IsNullOrEmpty(context.Message.UserId.ToString()))
                {
                    if (int.TryParse(context.Message.UserId.ToString(), out int parsedUserId))
                    {
                        userIdInt = parsedUserId;
                    }
                }

                if (context.Message != null && context.Message.TargetId != null)
                {
                    if (int.TryParse(context.Message.TargetId.ToString(), out int parsedTargetId))
                    {
                        targetIdInt = parsedTargetId;
                    }
                }

                DateTimeOffset eventTime = DateTimeOffset.FromUnixTimeSeconds(context.Message.Time);
                await logRepository.InsertOneAsync(new FiveMLog
                {
                    UserId = userIdInt,
                    Payload = context.Message.Message,
                    Room = context.Message.Room,
                    TargetId = targetIdInt,
                    Category = context.Message.Category,
                    EventTime = eventTime.UtcDateTime,
                });
                

                Log.Information("[{CityName}] | User_id: {user_id} | Target_id: {target_id} | Room: {room} | Category: {category} | Event_time: {event_time}",
                    citySettings.CityName, context.Message.UserId, context.Message.TargetId, context.Message.Room, context.Message.Category, DateTimeOffset.FromUnixTimeSeconds(context.Message.Time).UtcDateTime);
            }
            catch (Exception e)
            {
                throw new Exception(context.Message.ToString());

            }

            await Task.CompletedTask;
        }

    }
}

