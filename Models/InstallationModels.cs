namespace IzSetup.Models;

/// <summary>
/// Enum for installation status
/// </summary>
public enum InstallationStatus
{
    Pending,
    InProgress,
    Success,
    Failed,
    Cancelled
}

/// <summary>
/// Enum for log entry levels
/// </summary>
public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error,
    Success
}

/// <summary>
/// Represents the progress of a single software installation
/// </summary>
public class InstallationProgress
{
    /// <summary>
    /// Software item being installed
    /// </summary>
    public SoftwareItem Software { get; set; } = new();

    /// <summary>
    /// Current status of installation
    /// </summary>
    public InstallationStatus Status { get; set; } = InstallationStatus.Pending;

    /// <summary>
    /// Progress percentage (0-100)
    /// </summary>
    public int Percentage { get; set; }

    /// <summary>
    /// Any error message if installation failed
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Time when installation started
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Time when installation completed
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Duration of installation
    /// </summary>
    public TimeSpan Duration => EndTime > StartTime ? EndTime - StartTime : TimeSpan.Zero;
}

/// <summary>
/// Represents a single log entry
/// </summary>
public class LogEntry
{
    /// <summary>
    /// Timestamp of the log entry
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>
    /// Level of the log entry
    /// </summary>
    public LogLevel Level { get; set; } = LogLevel.Info;

    /// <summary>
    /// Message content
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Optional related software name
    /// </summary>
    public string? SoftwareName { get; set; }

    /// <summary>
    /// Optional exception details
    /// </summary>
    public string? Exception { get; set; }

    /// <summary>
    /// Formatted log line for display
    /// </summary>
    public string FormattedMessage =>
        $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level}] {Message}" +
        (string.IsNullOrEmpty(SoftwareName) ? "" : $" ({SoftwareName})");
}
