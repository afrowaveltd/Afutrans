using Afutrans.Core.Models.Settings;
using System.Text.Json;

namespace Afutrans.Core.Services;

public class SettingsService
{
   private readonly SemaphoreSlim _saveSemaphore = new(1, 1);
   private readonly Lock _settingsLock = new();
   private readonly string _settingsFilePath = Path.Combine(AppContext.BaseDirectory, "settings.json");
   private static readonly JsonSerializerOptions jsonOptions = new()
   {
      WriteIndented = true,
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
   };
   private ApplicationSettings _settings = new();
}
