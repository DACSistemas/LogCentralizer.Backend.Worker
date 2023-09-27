using LogCentralizer.Backend.Worker;
using LogCentralizer.Backend.Worker.Domain.Entities;
using LogCentralizer.Backend.Worker.Extensions;
using LogCentralizer.Backend.Worker.IoC;
using LogCentralizer.Backend.Worker.Providers;
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

        var citySettingsList = configuration.GetSection("CitySettingsList").Get<Dictionary<string, CitySettings>>();
        CitySettingsProvider.AllCitySettings = citySettingsList;

        services.AddMassTransit(x =>
        {
            x.AddConsumer<LogConsumer>();
            x.UsingRabbitMq((context, cfg) =>
                {
                    foreach (var citySettings in citySettingsList)
                    {
                        cfg.Host(citySettings.Value.RabbitMqSettings.Host, "/", h =>
                        {
                            h.Username(citySettings.Value.RabbitMqSettings.User);
                            h.Password(citySettings.Value.RabbitMqSettings.Password);
                        });
                        cfg.ReceiveEndpoint(citySettings.Key, e =>
                        {
                            e.Consumer<LogConsumer>(context);
                        });

                       
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
        
        ConfigureMongoDbService.Register(services, context.Configuration);
        //services.AddHostedService<Worker>();

        LogCitySettings(citySettingsList);

    }).UseSerilog()
    .Build();

async void LogCitySettings(Dictionary<string, CitySettings> citySettings)
{
    foreach (var settings in citySettings)
    {
        Log.Information("|--------------------------------------------------------------------------|");
        Log.Information("| City Name: {CityName}{space}|", settings.Value.CityName, String.Concat(Enumerable.Repeat(" ", (62 - settings.Value.CityName.Count()))));
        Log.Information("| RabbitMQ queue name: {QueueName}{space}|", settings.Key, String.Concat(Enumerable.Repeat(" ", (52 - settings.Key.Count()))));
        //Log.Information("| RabbitMQ Messages in queue: {MessageCount}{space}|", citySettings.MessageCount, String.Concat(Enumerable.Repeat(" ", (45 - citySettings.MessageCount.ToString().Length))));
        //Log.Information("| RabbitMQ Consumers: {ConsumerCount}{space}|", citySettings.ConsumerCount, String.Concat(Enumerable.Repeat(" ", (53 - citySettings.ConsumerCount.ToString().Length))));
        Log.Information("|--------------------------------------------------------------------------|");
    }
    
}

await host.RunAsync();
