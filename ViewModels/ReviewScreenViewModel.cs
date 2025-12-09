using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IzSetup.Models;
using IzSetup.Services;

namespace IzSetup.ViewModels;

/// <summary>
/// View model for review and confirmation screen
/// </summary>
public partial class ReviewScreenViewModel : ObservableObject
{
    private readonly SoftwareDataService _softwareDataService;
    private readonly LoggingService _loggingService;

    [ObservableProperty]
    private List<SoftwareItem> selectedSoftware = [];

    [ObservableProperty]
    private int totalApplications = 0;

    [ObservableProperty]
    private long totalSize = 0;

    [ObservableProperty]
    private string totalSizeFormatted = "0 B";

    [ObservableProperty]
    private string estimatedTime = "Calculating...";

    [ObservableProperty]
    private bool termsAgreed = false;

    public ReviewScreenViewModel(SoftwareDataService softwareDataService, LoggingService loggingService)
    {
        _softwareDataService = softwareDataService;
        _loggingService = loggingService;
    }

    /// <summary>
    /// Refresh selected software list and calculate totals
    /// </summary>
    public void RefreshSelectedSoftware()
    {
        SelectedSoftware = _softwareDataService.GetSelectedSoftware();
        TotalApplications = SelectedSoftware.Count;
        TotalSize = SelectedSoftware.Sum(s => s.Size);
        TotalSizeFormatted = FormatBytes(TotalSize);
        EstimatedTime = CalculateEstimatedTime();

        _loggingService.LogInfo($"Review screen updated: {TotalApplications} apps, {TotalSizeFormatted} total");
    }

    /// <summary>
    /// Calculate estimated installation time
    /// </summary>
    private string CalculateEstimatedTime()
    {
        // Estimate: ~5 seconds per app + ~10 MB/s download speed
        var appTime = TotalApplications * 5;
        var downloadTime = (TotalSize / 1024 / 1024) / 10;
        var totalSeconds = appTime + (int)downloadTime;

        if (totalSeconds < 60)
            return "< 1 minute";
        
        var minutes = totalSeconds / 60;
        if (minutes < 60)
            return $"~{minutes} minutes";
        
        var hours = minutes / 60;
        var remainingMinutes = minutes % 60;
        return $"~{hours}h {remainingMinutes}m";
    }

    /// <summary>
    /// Format bytes to human readable format
    /// </summary>
    private static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }

    /// <summary>
    /// Toggle terms agreement
    /// </summary>
    [RelayCommand]
    public void ToggleTermsAgreement()
    {
        TermsAgreed = !TermsAgreed;
    }
}
