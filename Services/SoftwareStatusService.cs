using IzSetup.Models;
using System.Diagnostics;

namespace IzSetup.Services;

/// <summary>
/// Service to check software installation status
/// </summary>
public class SoftwareStatusService
{
    private readonly LoggingService _loggingService;
    private static HashSet<string>? _installedAppsCache;
    private static bool _cacheLoaded = false;

    public SoftwareStatusService(LoggingService loggingService)
    {
        _loggingService = loggingService;
    }

    /// <summary>
    /// Load all installed apps once and cache them
    /// </summary>
    private async Task EnsureCacheLoadedAsync()
    {
        if (_cacheLoaded)
            return;

        try
        {
            var output = await RunCommandAsync("winget list");
            _installedAppsCache = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            
            foreach (var line in output.Split('\n'))
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    _installedAppsCache.Add(line.Trim());
                }
            }

            _cacheLoaded = true;
            _loggingService.LogInfo($"Cached {_installedAppsCache.Count} installed apps");
        }
        catch (Exception ex)
        {
            _loggingService.LogError("Failed to cache installed apps", ex);
            _installedAppsCache = new HashSet<string>();
            _cacheLoaded = true;
        }
    }

    /// <summary>
    /// Check if software is installed using cache
    /// </summary>
    public async Task<bool> IsInstalledAsync(string wingetId)
    {
        await EnsureCacheLoadedAsync();
        
        if (_installedAppsCache == null)
            return false;

        return _installedAppsCache.Any(line => line.Contains(wingetId, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Get installed version from cache
    /// </summary>
    public async Task<string?> GetInstalledVersionAsync(string wingetId)
    {
        await EnsureCacheLoadedAsync();
        
        if (_installedAppsCache == null)
            return null;

        var matchingLine = _installedAppsCache.FirstOrDefault(line => line.Contains(wingetId, StringComparison.OrdinalIgnoreCase));
        
        if (string.IsNullOrEmpty(matchingLine))
            return null;

        var parts = matchingLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 1)
        {
            return parts[1];
        }

        return null;
    }

    /// <summary>
    /// Check installation status for a software item using cached data
    /// </summary>
    public async Task CheckAndUpdateStatusAsync(SoftwareItem software)
    {
        if (string.IsNullOrEmpty(software.WingetId))
        {
            software.InstallationStatus = "NotInstalled";
            software.InstalledVersion = string.Empty;
            return;
        }

        try
        {
            var isInstalled = await IsInstalledAsync(software.WingetId);
            
            if (!isInstalled)
            {
                software.InstallationStatus = "NotInstalled";
                software.InstalledVersion = string.Empty;
            }
            else
            {
                var installedVersion = await GetInstalledVersionAsync(software.WingetId);
                software.InstalledVersion = installedVersion ?? string.Empty;
                
                // Simple version comparison - if installed version exists and is different, mark as UpdateAvailable
                if (!string.IsNullOrEmpty(installedVersion) && 
                    !string.IsNullOrEmpty(software.Version) && 
                    installedVersion != software.Version)
                {
                    software.InstallationStatus = "UpdateAvailable";
                }
                else
                {
                    software.InstallationStatus = "Installed";
                }
            }
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"Failed to check status for {software.Name}", ex);
            software.InstallationStatus = "NotInstalled";
            software.InstalledVersion = string.Empty;
        }
    }

    /// <summary>
    /// Check status for multiple software items
    /// </summary>
    public async Task CheckAndUpdateStatusAsync(List<SoftwareItem> softwareList)
    {
        foreach (var software in softwareList)
        {
            await CheckAndUpdateStatusAsync(software);
        }
    }

    /// <summary>
    /// Run command and get output
    /// </summary>
    private async Task<string> RunCommandAsync(string command)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "cmd",
            Arguments = $"/c {command}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = System.Text.Encoding.UTF8
        };

        using var process = Process.Start(psi);
        if (process == null)
            return string.Empty;

        var output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();
        
        return output;
    }
}
