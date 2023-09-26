using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace LogCentralizer.Backend.Worker.Extensions
{
    public static class SerilogExtension
    {
        public static void AddSerilog()
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()

            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information, theme: SystemConsoleTheme.Literate)
            .CreateLogger();
        }
    }
}
