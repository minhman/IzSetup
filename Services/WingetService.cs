using System.Diagnostics;
using System.Text.Json;
using IzSetup.Models;

namespace IzSetup.Services;

/// <summary>
/// Service for executing winget commands and managing software installations
/// </summary>
public class WingetService
{
    private readonly string _wingetPath;
    private readonly int _timeoutSeconds;
    private readonly LoggingService _loggingService;

    public WingetService(string wingetPath = "winget", int timeoutSeconds = 300, LoggingService? loggingService = null)
    {
        _wingetPath = wingetPath;
        _timeoutSeconds = timeoutSeconds;
        _loggingService = loggingService ?? new LoggingService();
    }

    /// <summary>
    /// Search for a software package in winget
    /// </summary>
    public async Task<bool> SearchPackageAsync(string packageId)
    {
        try
        {
            var result = await ExecuteWingetCommandAsync($"search {packageId}");
            return result.Success && result.Output.Contains(packageId);
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"Failed to search package: {packageId}", ex);
            return false;
        }
    }

    /// <summary>
    /// Install a single software package
    /// </summary>
    public async Task<bool> InstallPackageAsync(string packageId, string? scope = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var command = $"install {packageId} --silent --accept-package-agreements --accept-source-agreements";
            
            if (!string.IsNullOrEmpty(scope))
            {
                command += $" --scope {scope}";
            }

            var result = await ExecuteWingetCommandAsync(command, cancellationToken);
            
            if (result.Success)
            {
                _loggingService.LogSuccess($"Successfully installed: {packageId}");
            }
            else
            {
                _loggingService.LogError($"Failed to install {packageId}: {result.Error}");
            }

            return result.Success;
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"Exception installing {packageId}", ex);
            return false;
        }
    }

    /// <summary>
    /// Install multiple software packages sequentially
    /// </summary>
    public async Task<(int successful, int failed, List<string> failedPackages)> InstallMultipleAsync(
        IEnumerable<string> packageIds,
        string? scope = null,
        IProgress<(int total, int completed, string current)>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var packages = packageIds.ToList();
        var successful = 0;
        var failed = 0;
        var failedPackages = new List<string>();

        for (int i = 0; i < packages.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var packageId = packages[i];
            progress?.Report((packages.Count, i, packageId));

            if (await InstallPackageAsync(packageId, scope, cancellationToken))
            {
                successful++;
            }
            else
            {
                failed++;
                failedPackages.Add(packageId);
            }
        }

        _loggingService.LogInfo($"Installation batch completed: {successful} successful, {failed} failed");
        return (successful, failed, failedPackages);
    }

    /// <summary>
    /// Get information about a package
    /// </summary>
    public async Task<Dictionary<string, string>> GetPackageInfoAsync(string packageId)
    {
        var info = new Dictionary<string, string>();
        try
        {
            var result = await ExecuteWingetCommandAsync($"show {packageId} --json");
            if (result.Success)
            {
                try
                {
                    var json = JsonDocument.Parse(result.Output);
                    var data = json.RootElement.GetProperty("Data");
                    
                    if (data.TryGetProperty("Publisher", out var publisher))
                        info["Publisher"] = publisher.GetString() ?? "";
                    if (data.TryGetProperty("Version", out var version))
                        info["Version"] = version.GetString() ?? "";
                    if (data.TryGetProperty("Description", out var desc))
                        info["Description"] = desc.GetString() ?? "";
                    if (data.TryGetProperty("Homepage", out var homepage))
                        info["Homepage"] = homepage.GetString() ?? "";
                }
                catch
                {
                    _loggingService.LogWarning($"Failed to parse JSON for {packageId}");
                }
            }
        }
        catch (Exception ex)
        {
            _loggingService.LogError($"Failed to get info for {packageId}", ex);
        }

        return info;
    }

    /// <summary>
    /// List all installed packages
    /// </summary>
    public async Task<List<string>> ListInstalledPackagesAsync()
    {
        var packages = new List<string>();
        try
        {
            var result = await ExecuteWingetCommandAsync("list");
            if (result.Success)
            {
                var lines = result.Output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                packages.AddRange(lines.Skip(2).Select(line => line.Trim()).Where(l => !string.IsNullOrEmpty(l)));
            }
        }
        catch (Exception ex)
        {
            _loggingService.LogError("Failed to list installed packages", ex);
        }

        return packages;
    }

    /// <summary>
    /// Execute a raw winget command
    /// </summary>
    private async Task<(bool Success, string Output, string Error)> ExecuteWingetCommandAsync(
        string arguments,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(_timeoutSeconds));

            var psi = new ProcessStartInfo
            {
                FileName = _wingetPath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null)
            {
                return (false, "", "Failed to start winget process");
            }

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            await Task.WhenAll(outputTask, errorTask);

            await process.WaitForExitAsync(cts.Token);

            return (process.ExitCode == 0, outputTask.Result, errorTask.Result);
        }
        catch (OperationCanceledException)
        {
            _loggingService.LogError("Winget command timeout", null);
            return (false, "", "Command timeout");
        }
        catch (Exception ex)
        {
            _loggingService.LogError("Winget command error", ex);
            return (false, "", ex.Message);
        }
    }
}
