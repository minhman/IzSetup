using CommunityToolkit.Mvvm.ComponentModel;

namespace IzSetup.Models;

/// <summary>
/// Represents a software item that can be installed via winget
/// </summary>
public partial class SoftwareItem : ObservableObject
{
    /// <summary>
    /// Unique identifier for the software
    /// </summary>
    [ObservableProperty]
    private string id = string.Empty;

    /// <summary>
    /// Display name of the software
    /// </summary>
    [ObservableProperty]
    private string name = string.Empty;

    /// <summary>
    /// Software publisher/manufacturer
    /// </summary>
    [ObservableProperty]
    private string publisher = string.Empty;

    /// <summary>
    /// Current version of the software
    /// </summary>
    [ObservableProperty]
    private string version = string.Empty;

    /// <summary>
    /// Category of the software (e.g., "Development", "Productivity")
    /// </summary>
    [ObservableProperty]
    private string category = string.Empty;

    /// <summary>
    /// Brief description of the software
    /// </summary>
    [ObservableProperty]
    private string description = string.Empty;

    /// <summary>
    /// Estimated download size in bytes
    /// </summary>
    [ObservableProperty]
    private long size;

    /// <summary>
    /// Website URL of the software
    /// </summary>
    [ObservableProperty]
    private string url = string.Empty;

    /// <summary>
    /// License type (e.g., "Free", "Open Source", "Commercial")
    /// </summary>
    [ObservableProperty]
    private string license = string.Empty;

    /// <summary>
    /// Whether this item is selected for installation
    /// </summary>
    [ObservableProperty]
    private bool isSelected;

    /// <summary>
    /// Winget package identifier (e.g., "Microsoft.PowerToys")
    /// </summary>
    [ObservableProperty]
    private string wingetId = string.Empty;

    /// <summary>
    /// Installation scope: "User" or "System"
    /// </summary>
    [ObservableProperty]
    private string scope = "User";

    /// <summary>
    /// Installation status: NotInstalled, Installed, UpdateAvailable
    /// </summary>
    [ObservableProperty]
    private string installationStatus = "NotInstalled"; // NotInstalled, Installed, UpdateAvailable

    /// <summary>
    /// Currently installed version (if any)
    /// </summary>
    [ObservableProperty]
    private string installedVersion = string.Empty;
}
