using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IzSetup.Models;
using IzSetup.Services;

namespace IzSetup.ViewModels;

/// <summary>
/// View model for installation progress screen
/// </summary>
public partial class InstallationScreenViewModel : ObservableObject
{
    private readonly SoftwareDataService _softwareDataService;
    private readonly SettingsService _settingsService;
    private readonly LoggingService _loggingService;
    private readonly WingetService _wingetService;
    private CancellationTokenSource? _cancellationTokenSource;

    [ObservableProperty]
    private List<InstallationProgress> installations = [];

    [ObservableProperty]
    private List<LogEntry> logs = [];

    [ObservableProperty]
    private int overallProgress = 0;

    [ObservableProperty]
    private string overallProgressText = "0%";

    [ObservableProperty]
    private int completedCount = 0;

    [ObservableProperty]
    private int totalCount = 0;

    [ObservableProperty]
    private bool isInstalling = false;

    [ObservableProperty]
    private string statusMessage = "Ready";

    [ObservableProperty]
    private bool canCancel = true;

    [ObservableProperty]
    private bool canFinish = false;

    public InstallationScreenViewModel(SoftwareDataService softwareDataService, SettingsService settingsService, LoggingService loggingService)
    {
        _softwareDataService = softwareDataService;
        _settingsService = settingsService;
        _loggingService = loggingService;

        var settings = _settingsService.GetSettings();
        _wingetService = new WingetService(settings.WingetPath, settings.InstallationTimeoutSeconds, _loggingService);

        _loggingService.LogEntryAdded += (s, e) => Logs = new List<LogEntry>(_loggingService.GetAllLogs());
    }

    /// <summary>
    /// Start installation of selected software
    /// </summary>
    public void StartInstallation(List<SoftwareItem> softwareToInstall)
    {
        if (softwareToInstall.Count == 0)
        {
            StatusMessage = "No software selected";
            return;
        }

        IsInstalling = true;
        CanCancel = true;
        CanFinish = false;
        CompletedCount = 0;
        TotalCount = softwareToInstall.Count;
        Installations = softwareToInstall.Select(s => new InstallationProgress { Software = s }).ToList();

        _cancellationTokenSource = new CancellationTokenSource();
        _ = InstallAsync(softwareToInstall, _cancellationTokenSource.Token);
    }

    /// <summary>
    /// Perform the actual installation
    /// </summary>
    private async Task InstallAsync(List<SoftwareItem> softwareToInstall, CancellationToken cancellationToken)
    {
        try
        {
            var settings = _settingsService.GetSettings();
            var progress = new Progress<(int total, int completed, string current)>(tuple =>
            {
                OverallProgress = (tuple.completed * 100) / TotalCount;
                OverallProgressText = $"{OverallProgress}%";
                StatusMessage = $"Installing {tuple.current}...";
            });

            var packageIds = softwareToInstall.Select(s => s.WingetId).ToList();
            var (successful, failed, failedPackages) = await _wingetService.InstallMultipleAsync(
                packageIds,
                settings.InstallationScope,
                progress,
                cancellationToken);

            CompletedCount = successful;
            OverallProgress = 100;
            OverallProgressText = "100%";

            if (failed == 0)
            {
                StatusMessage = $"✓ Successfully installed {successful} applications";
                _loggingService.LogSuccess($"Installation complete: {successful}/{TotalCount} success");
            }
            else
            {
                StatusMessage = $"✓ Installed {successful}/{TotalCount} (Failed: {failed})";
                _loggingService.LogWarning($"Installation completed with {failed} failures");
            }

            IsInstalling = false;
            CanCancel = false;
            CanFinish = true;

            // Update installation statuses
            for (int i = 0; i < Installations.Count; i++)
            {
                Installations[i].Status = failedPackages.Contains(Installations[i].Software.WingetId) 
                    ? InstallationStatus.Failed 
                    : InstallationStatus.Success;
                Installations[i].Percentage = 100;
            }
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Installation cancelled";
            IsInstalling = false;
            CanCancel = false;
            CanFinish = true;
            _loggingService.LogInfo("Installation cancelled by user");
        }
        catch (Exception ex)
        {
            StatusMessage = "Installation failed";
            IsInstalling = false;
            CanCancel = false;
            CanFinish = true;
            _loggingService.LogError("Installation error", ex);
        }
    }

    /// <summary>
    /// Cancel installation
    /// </summary>
    [RelayCommand]
    public void CancelInstallation()
    {
        _cancellationTokenSource?.Cancel();
    }

    /// <summary>
    /// Reset installation state
    /// </summary>
    public void Reset()
    {
        Installations.Clear();
        Logs.Clear();
        OverallProgress = 0;
        OverallProgressText = "0%";
        CompletedCount = 0;
        TotalCount = 0;
        IsInstalling = false;
        CanCancel = true;
        CanFinish = false;
        StatusMessage = "Ready";
    }
}
