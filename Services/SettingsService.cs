using System.Text.Json;
using IzSetup.Models;

namespace IzSetup.Services;

/// <summary>
/// Service for managing application settings
/// </summary>
public class SettingsService
{
    private AppSettings _settings = new();
    private readonly string _settingsPath;
    private readonly LoggingService _loggingService;

    public SettingsService(string settingsPath = "appsettings.json", LoggingService? loggingService = null)
    {
        _settingsPath = settingsPath;
        _loggingService = loggingService ?? new LoggingService();
    }

    /// <summary>
    /// Load settings from JSON file
    /// </summary>
    public async Task<bool> LoadSettingsAsync()
    {
        try
        {
            if (!File.Exists(_settingsPath))
            {
                _loggingService.LogInfo("Settings file not found, using defaults");
                _settings = new AppSettings();
                await SaveSettingsAsync();
                return true;
            }

            var json = await File.ReadAllTextAsync(_settingsPath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _settings = JsonSerializer.Deserialize<AppSettings>(json, options) ?? new AppSettings();

            _loggingService.LogInfo("Settings loaded successfully");
            return true;
        }
        catch (Exception ex)
        {
            _loggingService.LogError("Failed to load settings", ex);
            _settings = new AppSettings();
            return false;
        }
    }

    /// <summary>
    /// Save settings to JSON file
    /// </summary>
    public async Task<bool> SaveSettingsAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_settingsPath, json);

            _loggingService.LogInfo("Settings saved successfully");
            return true;
        }
        catch (Exception ex)
        {
            _loggingService.LogError("Failed to save settings", ex);
            return false;
        }
    }

    /// <summary>
    /// Get current settings
    /// </summary>
    public AppSettings GetSettings() => _settings;

    /// <summary>
    /// Update a specific setting
    /// </summary>
    public void UpdateSetting<T>(string propertyName, T value)
    {
        var property = typeof(AppSettings).GetProperty(propertyName);
        if (property != null && property.CanWrite)
        {
            property.SetValue(_settings, value);
        }
    }

    /// <summary>
    /// Get a specific setting value
    /// </summary>
    public T? GetSetting<T>(string propertyName)
    {
        var property = typeof(AppSettings).GetProperty(propertyName);
        if (property != null && property.CanRead)
        {
            return (T?)property.GetValue(_settings);
        }
        return default;
    }

    /// <summary>
    /// Reset settings to defaults
    /// </summary>
    public async Task ResetToDefaultsAsync()
    {
        _settings = new AppSettings();
        await SaveSettingsAsync();
        _loggingService.LogInfo("Settings reset to defaults");
    }
}
