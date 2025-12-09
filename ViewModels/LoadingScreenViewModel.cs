using CommunityToolkit.Mvvm.ComponentModel;
using IzSetup.Services;
using IzSetup.Models;

namespace IzSetup.ViewModels;

/// <summary>
/// View model for loading/checking software status screen
/// </summary>
public partial class LoadingScreenViewModel : ObservableObject
{
    private readonly SoftwareDataService _softwareDataService;
    private readonly SoftwareStatusService _softwareStatusService;
    private readonly LoggingService _loggingService;

    [ObservableProperty]
    private string statusMessage = "Loading software list...";

    [ObservableProperty]
    private int progressValue = 0;

    [ObservableProperty]
    private string progressText = "0/0";

    public event Action? CheckCompleted;

    public LoadingScreenViewModel(SoftwareDataService softwareDataService, SoftwareStatusService softwareStatusService, LoggingService loggingService)
    {
        _softwareDataService = softwareDataService;
        _softwareStatusService = softwareStatusService;
        _loggingService = loggingService;
    }

    /// <summary>
    /// Load software and check installation status (with parallel checking for speed)
    /// </summary>
    public async Task<List<SoftwareItem>> LoadAndCheckSoftwareAsync()
    {
        try
        {
            // Load software list
            StatusMessage = "Loading software list...";
            ProgressValue = 0;
            ProgressText = "0/0";

            await _softwareDataService.LoadSoftwareListAsync();
            var allSoftware = _softwareDataService.GetAllSoftware();

            // Check installation status in parallel (max 5 at a time for speed)
            StatusMessage = "Checking installation status...";
            int total = allSoftware.Count;
            
            // Use parallel processing with max 5 concurrent tasks
            var semaphore = new System.Threading.SemaphoreSlim(5);
            var checkTasks = new List<Task>();

            for (int i = 0; i < total; i++)
            {
                var software = allSoftware[i];
                var index = i;

                var task = Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        await _softwareStatusService.CheckAndUpdateStatusAsync(software);
                        
                        // Update progress on UI thread
                        var completed = allSoftware.Count(s => !string.IsNullOrEmpty(s.InstallationStatus));
                        StatusMessage = $"Checking {software.Name}...";
                        ProgressValue = (int)((completed * 100.0) / total);
                        ProgressText = $"{completed}/{total}";
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                checkTasks.Add(task);
            }

            // Wait for all checks to complete
            await Task.WhenAll(checkTasks);

            StatusMessage = "Ready!";
            ProgressValue = 100;
            ProgressText = $"{total}/{total}";

            _loggingService.LogInfo($"Status check completed for {total} items");
            
            // Delay a bit then trigger transition
            await Task.Delay(500);
            CheckCompleted?.Invoke();

            return allSoftware;
        }
        catch (Exception ex)
        {
            _loggingService.LogError("Failed to load and check software", ex);
            StatusMessage = "Error occurred!";
            return [];
        }
    }
}
