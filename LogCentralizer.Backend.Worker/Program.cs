using LogCentralizer.Backend.Repository.Worker.MongoDB.Entities;
using LogCentralizer.Backend.Worker;
using LogCentralizer.Backend.Worker.Domain.Entities;
using LogCentralizer.Backend.Worker.Extensions;
using LogCentralizer.Backend.Worker.Providers;
using LogCentralizer.Backend.Worker.Repository.MongoDB.Repositories;
using LogCentralizer.BackEnd.Worker.Repository.MongoDB.Repositories;
using MassTransit;
using MongoDB.Driver;
using Serilog;


var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory()) // Set the base path for configuration files
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Load appsettings.json
    .Build();


SerilogExtension.AddSerilog();
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {

        services.Configure<List<CitySettings>>(configuration.GetSection("CitySettingsList"));

        var citySettingsList = configuration.GetSection("CitySettingsList").Get<List<CitySettings>>();
        CitySettingsProvider.AllCitySettings.AddRange(citySettingsList);
       
        //InitializeLogRepositories(citySettingsList);
        
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
                {
                    foreach (var citySettings in citySettingsList)
                    {
                        cfg.ReceiveEndpoint(citySettings.QueueName, e =>
                        {
                            e.Consumer<LogConsumer>();
                        });

                        LogCitySettings(citySettings);
                    }
                    Thread.Sleep(2000);

                    cfg.ClearSerialization();
                    cfg.UseDelayedMessageScheduler();
                    cfg.UseRawJsonSerializer();
                    cfg.ConfigureEndpoints(context);
                });

            x.SetJobConsumerOptions();
            x.SetKebabCaseEndpointNameFormatter();
            
        });
                        
        services.AddOptions<MassTransitHostOptions>()
            .Configure(options =>
            {
                options.WaitUntilStarted = true;
                options.StartTimeout = TimeSpan.FromMinutes(1);
                options.StopTimeout = TimeSpan.FromMinutes(1);
            });

        services.AddOptions<HostOptions>()
            .Configure(options => options.ShutdownTimeout = TimeSpan.FromMinutes(1));

        //services.AddHostedService<Worker>();

    }).UseSerilog()
    .Build();

async void LogCitySettings(CitySettings citySettings)
{
    Log.Information("|--------------------------------------------------------------------------|");
    Log.Information("| City Name: {CityName}{space}|", citySettings.CityName, String.Concat(Enumerable.Repeat(" ", (62 - citySettings.CityName.Count()))));
    Log.Information("| RabbitMQ queue name: {QueueName}{space}|", citySettings.QueueName, String.Concat(Enumerable.Repeat(" ", (52 - citySettings.QueueName.Count()))));
    //Log.Information("| RabbitMQ Messages in queue: {MessageCount}{space}|", citySettings.MessageCount, String.Concat(Enumerable.Repeat(" ", (45 - citySettings.MessageCount.ToString().Length))));
    //Log.Information("| RabbitMQ Consumers: {ConsumerCount}{space}|", citySettings.ConsumerCount, String.Concat(Enumerable.Repeat(" ", (53 - citySettings.ConsumerCount.ToString().Length))));
    Log.Information("|--------------------------------------------------------------------------|");
}


Dictionary<string, IMongoRepository<FiveMLog>> InitializeLogRepositories(List<CitySettings> citiesSettings)
{
    var logRepositories = new Dictionary<string, IMongoRepository<FiveMLog>>();

    foreach (var citySettings in citiesSettings)
    {
        // Configurar uma instância de IMongoRepository para cada configuração de MongoDB
        var settings = citySettings.MongoDbSettings;
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);

        var repository = new MongoRepository<FiveMLog>(database);

        // Adicionar o repositório à lista de repositórios
        logRepositories[citySettings.CityName] = repository;
    }

    return logRepositories;
}


await host.RunAsync();
