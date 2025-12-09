namespace IzSetup.Helpers;

/// <summary>
/// Helper class for time-related utilities
/// </summary>
public static class TimeHelper
{
    /// <summary>
    /// Format duration to human readable format
    /// </summary>
    public static string FormatDuration(TimeSpan duration)
    {
        if (duration.TotalSeconds < 60)
            return $"{(int)duration.TotalSeconds}s";
        
        if (duration.TotalMinutes < 60)
            return $"{(int)duration.TotalMinutes}m {duration.Seconds}s";
        
        return $"{(int)duration.TotalHours}h {duration.Minutes}m";
    }

    /// <summary>
    /// Get estimated time string
    /// </summary>
    public static string GetEstimatedTime(long totalBytes)
    {
        // Assume ~10 MB/s download speed
        var seconds = totalBytes / 1024 / 1024 / 10;
        
        if (seconds < 60)
            return "< 1 minute";
        
        var minutes = seconds / 60;
        if (minutes < 60)
            return $"~{minutes} minutes";
        
        var hours = minutes / 60;
        return $"~{hours}h {minutes % 60}m";
    }
}

/// <summary>
/// Helper class for file size formatting
/// </summary>
public static class FileSizeHelper
{
    /// <summary>
    /// Format bytes to human readable format
    /// </summary>
    public static string FormatBytes(long bytes)
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
    /// Parse bytes from human readable format
    /// </summary>
    public static long ParseBytes(string input)
    {
        var parts = input.Split(' ');
        if (parts.Length != 2)
            return 0;

        if (!double.TryParse(parts[0], out var value))
            return 0;

        return parts[1].ToUpper() switch
        {
            "B" => (long)value,
            "KB" => (long)(value * 1024),
            "MB" => (long)(value * 1024 * 1024),
            "GB" => (long)(value * 1024 * 1024 * 1024),
            "TB" => (long)(value * 1024 * 1024 * 1024 * 1024),
            _ => 0
        };
    }
}

/// <summary>
/// Helper class for validation
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Check if string is valid software name
    /// </summary>
    public static bool IsValidSoftwareName(string name)
    {
        return !string.IsNullOrWhiteSpace(name) && name.Length <= 255;
    }

    /// <summary>
    /// Check if string is valid winget package ID
    /// </summary>
    public static bool IsValidWingetId(string id)
    {
        return !string.IsNullOrWhiteSpace(id) && id.Contains(".");
    }

    /// <summary>
    /// Check if category is valid
    /// </summary>
    public static bool IsValidCategory(string category)
    {
        var validCategories = new[] { "Development", "Utilities", "Media", "Browsers", "Office", "Communication", "Games", "Other" };
        return validCategories.Contains(category);
    }
}

/// <summary>
/// Helper class for string operations
/// </summary>
public static class StringHelper
{
    /// <summary>
    /// Truncate string to specified length
    /// </summary>
    public static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return value.Length <= maxLength ? value : value[..maxLength] + "...";
    }

    /// <summary>
    /// Check if string is a valid URL
    /// </summary>
    public static bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    /// <summary>
    /// Sanitize string for file operations
    /// </summary>
    public static string SanitizeForFilename(string filename)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        foreach (var c in invalidChars)
        {
            filename = filename.Replace(c, '_');
        }
        return filename;
    }
}
