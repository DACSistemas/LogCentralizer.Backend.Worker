using LogCentralizer.Backend.Worker.Domain.Entities;

namespace LogCentralizer.Backend.Worker.Providers
{
    public static class CitySettingsProvider
    {
        // Crie uma lista estática de CitySettings
        public static List<CitySettings> AllCitySettings { get; } = new List<CitySettings>();
    }
}
