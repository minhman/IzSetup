using IzSetup.Models;

namespace IzSetup.Services;

/// <summary>
/// Service for application logging with file persistence
/// </summary>
public class LoggingService
{
    private readonly List<LogEntry> _logs = [];
    private readonly string _logsDirectory;
    private readonly bool _enableFileLogging;

    public event EventHandler<LogEntry>? LogEntryAdded;

    public LoggingService(string logsDirectory = "Logs", bool enableFileLogging = true)
    {
        _logsDirectory = logsDirectory;
        _enableFileLogging = enableFileLogging;

        if (_enableFileLogging && !Directory.Exists(_logsDirectory))
        {
            Directory.CreateDirectory(_logsDirectory);
        }
    }

    /// <summary>
    /// Get all logs
    /// </summary>
    public List<LogEntry> GetAllLogs() => new(_logs);

    /// <summary>
    /// Get logs by level
    /// </summary>
    public List<LogEntry> GetLogsByLevel(LogLevel level) =>
        _logs.Where(l => l.Level == level).ToList();

    /// <summary>
    /// Get recent logs
    /// </summary>
    public List<LogEntry> GetRecentLogs(int count) =>
        _logs.TakeLast(count).ToList();

    /// <summary>
    /// Log information message
    /// </summary>
    public void LogInfo(string message, string? softwareName = null) =>
        AddLog(new LogEntry { Level = LogLevel.Info, Message = message, SoftwareName = softwareName });

    /// <summary>
    /// Log debug message
    /// </summary>
    public void LogDebug(string message, string? softwareName = null) =>
        AddLog(new LogEntry { Level = LogLevel.Debug, Message = message, SoftwareName = softwareName });

    /// <summary>
    /// Log warning message
    /// </summary>
    public void LogWarning(string message, string? softwareName = null) =>
        AddLog(new LogEntry { Level = LogLevel.Warning, Message = message, SoftwareName = softwareName });

    /// <summary>
    /// Log error message
    /// </summary>
    public void LogError(string message, Exception? exception = null, string? softwareName = null)
    {
        var entry = new LogEntry
        {
            Level = LogLevel.Error,
            Message = message,
            Exception = exception?.ToString(),
            SoftwareName = softwareName
        };
        AddLog(entry);
    }

    /// <summary>
    /// Log success message
    /// </summary>
    public void LogSuccess(string message, string? softwareName = null) =>
        AddLog(new LogEntry { Level = LogLevel.Success, Message = message, SoftwareName = softwareName });

    /// <summary>
    /// Add a log entry
    /// </summary>
    private void AddLog(LogEntry entry)
    {
        _logs.Add(entry);
        LogEntryAdded?.Invoke(this, entry);

        if (_enableFileLogging)
        {
            WriteToFileAsync(entry).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Write log entry to file
    /// </summary>
    private async Task WriteToFileAsync(LogEntry entry)
    {
        try
        {
            var fileName = $"Log_{DateTime.Now:yyyy-MM-dd}.txt";
            var filePath = Path.Combine(_logsDirectory, fileName);

            var logLine = entry.FormattedMessage;
            if (!string.IsNullOrEmpty(entry.Exception))
            {
                logLine += $"\n{entry.Exception}";
            }

            await File.AppendAllTextAsync(filePath, logLine + Environment.NewLine);
        }
        catch
        {
            // Silently fail file logging to not break the application
        }
    }

    /// <summary>
    /// Clear all logs
    /// </summary>
    public void ClearLogs() => _logs.Clear();

    /// <summary>
    /// Clear logs older than specified days
    /// </summary>
    public async Task ClearOldLogsAsync(int days)
    {
        try
        {
            var cutoffDate = DateTime.Now.AddDays(-days);
            var directory = new DirectoryInfo(_logsDirectory);

            foreach (var file in directory.GetFiles("Log_*.txt"))
            {
                if (file.LastWriteTime < cutoffDate)
                {
                    file.Delete();
                }
            }
        }
        catch (Exception ex)
        {
            LogError("Failed to clear old logs", ex);
        }
    }

    /// <summary>
    /// Export logs to file
    /// </summary>
    public async Task<bool> ExportLogsAsync(string exportPath)
    {
        try
        {
            var content = string.Join(Environment.NewLine, _logs.Select(l => l.FormattedMessage));
            await File.WriteAllTextAsync(exportPath, content);
            return true;
        }
        catch (Exception ex)
        {
            LogError("Failed to export logs", ex);
            return false;
        }
    }
}
