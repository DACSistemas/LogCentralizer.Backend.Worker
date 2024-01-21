using LogCentralizer.Backend.Worker;
using LogCentralizer.Backend.Worker.Domain.Entities;
using LogCentralizer.Backend.Worker.Extensions;
using LogCentralizer.Backend.Worker.IoC;
using LogCentralizer.Backend.Worker.Providers;
using MassTransit;
using Serilog;

//var configuration = new ConfigurationBuilder()
//    .SetBasePath(Directory.GetCurrentDirectory()) // Set the base path for configuration files
//    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Load appsettings.json
//    .Build();

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings{(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" ? ".Development" : "")}.json", 
    optional: false, reloadOnChange: true)
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

        LogCitySettings(citySettingsList);

    }).UseSerilog()
    .Build();

static void LogCitySettings(Dictionary<string, CitySettings> citySettings)
{
    Log.Information(@"
         _   ___     ___          |========================================================|
        | | / \ \   / / \         | - Módulo desenvolvido por Diego Ansanello Cataldi      |
     _  | |/ _ \ \ / / _ \        | - Contato: (19) 9.7419-6348                            |
    | |_| / ___ \ V / ___ \       | - Discord: javascripter                                |
     \___/_/   \_\_/_/   \_\      |========================================================|
    ");
    Thread.Sleep(2000);

    Log.Information("{Queues}:", "Filas Encontradas");
    foreach (var settings in citySettings)
    {
        int desiredLength = 60;
        string paddedQueueName = settings.Key.Length <= desiredLength
            ? settings.Key
            : settings.Key.Substring(0, desiredLength);

        
        Log.Information($"City Name: {settings.Value.CityName}");
        Log.Information($"RabbitMQ queue name: {paddedQueueName}");
        
    }
    //Log.Information(Environment.NewLine);
}

await host.RunAsync();
