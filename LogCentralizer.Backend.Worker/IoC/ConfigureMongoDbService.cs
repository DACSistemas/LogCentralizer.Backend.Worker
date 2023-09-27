using LogCentralizer.Backend.Worker.Repository.MongoDB.Repositories;
using LogCentralizer.BackEnd.Worker.Repository.MongoDB.Repositories;
using LogCentralizer.BackEnd.Worker.Repository.MongoDB.Settings;
using Microsoft.Extensions.Options;

namespace LogCentralizer.Backend.Worker.IoC
{
    public static class ConfigureMongoDbService
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

            services.AddSingleton<IMongoDbSettings>(serviceProvider =>
                serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value);

            services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        }
    }
}
