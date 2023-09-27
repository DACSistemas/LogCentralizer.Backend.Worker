using LogCentralizer.Backend.Worker.Domain.Entities;

namespace LogCentralizer.Backend.Worker.Providers
{
    public static class CitySettingsProvider
    {
        public static Dictionary<string, CitySettings> AllCitySettings { get; set; }
    }
}
