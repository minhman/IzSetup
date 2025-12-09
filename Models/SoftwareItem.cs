namespace IzSetup.Models;

/// <summary>
/// Represents a software item that can be installed via winget
/// </summary>
public class SoftwareItem
{
    /// <summary>
    /// Unique identifier for the software
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the software
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Software publisher/manufacturer
    /// </summary>
    public string Publisher { get; set; } = string.Empty;

    /// <summary>
    /// Current version of the software
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Category of the software (e.g., "Development", "Productivity")
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Brief description of the software
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Estimated download size in bytes
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Website URL of the software
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// License type (e.g., "Free", "Open Source", "Commercial")
    /// </summary>
    public string License { get; set; } = string.Empty;

    /// <summary>
    /// Whether this item is selected for installation
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// Winget package identifier (e.g., "Microsoft.PowerToys")
    /// </summary>
    public string WingetId { get; set; } = string.Empty;

    /// <summary>
    /// Installation scope: "User" or "System"
    /// </summary>
    public string Scope { get; set; } = "User";
}
