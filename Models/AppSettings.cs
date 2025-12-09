namespace IzSetup.Models;

/// <summary>
/// Application-wide settings and configuration
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Application language code (e.g., "en-US", "vi-VN")
    /// </summary>
    public string Language { get; set; } = "en-US";

    /// <summary>
    /// Enable dark mode theme
    /// </summary>
    public bool DarkMode { get; set; } = false;

    /// <summary>
    /// Path to winget executable
    /// </summary>
    public string WingetPath { get; set; } = "winget";

    /// <summary>
    /// Installation timeout in seconds
    /// </summary>
    public int InstallationTimeoutSeconds { get; set; } = 300;

    /// <summary>
    /// Enable detailed logging
    /// </summary>
    public bool EnableDetailedLogging { get; set; } = true;

    /// <summary>
    /// Installation scope preference: "User" or "System"
    /// </summary>
    public string InstallationScope { get; set; } = "User";

    /// <summary>
    /// Path to software list JSON file
    /// </summary>
    public string SoftwareListPath { get; set; } = "Data/software_list.json";

    /// <summary>
    /// Path to application settings JSON file
    /// </summary>
    public string SettingsPath { get; set; } = "appsettings.json";

    /// <summary>
    /// Path to logs directory
    /// </summary>
    public string LogsDirectory { get; set; } = "Logs";

    /// <summary>
    /// Retain installation logs for N days
    /// </summary>
    public int LogRetentionDays { get; set; } = 30;

    /// <summary>
    /// Automatically check for updates
    /// </summary>
    public bool AutoCheckUpdates { get; set; } = true;

    /// <summary>
    /// Install pre-release versions
    /// </summary>
    public bool AllowPrerelease { get; set; } = false;

    /// <summary>
    /// Agree to terms of use
    /// </summary>
    public bool TermsAgreed { get; set; } = false;
}
