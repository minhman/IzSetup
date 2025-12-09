using System.Text.Json;
using IzSetup.Models;

namespace IzSetup.Services;

/// <summary>
/// Service for managing software data from JSON file
/// </summary>
public class SoftwareDataService
{
    private List<SoftwareItem> _softwareList = [];
    private readonly string _dataPath;
    private readonly LoggingService _loggingService;

    public SoftwareDataService(string dataPath = "Data/software_list.json", LoggingService? loggingService = null)
    {
        _dataPath = dataPath;
        _loggingService = loggingService ?? new LoggingService();
    }

    /// <summary>
    /// Load software list from JSON file
    /// </summary>
    public async Task<bool> LoadSoftwareListAsync()
    {
        try
        {
            if (!File.Exists(_dataPath))
            {
                _loggingService.LogWarning($"Software list not found at {_dataPath}, creating default list");
                await CreateDefaultListAsync();
                return true;
            }

            var json = await File.ReadAllTextAsync(_dataPath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _softwareList = JsonSerializer.Deserialize<List<SoftwareItem>>(json, options) ?? [];

            _loggingService.LogInfo($"Loaded {_softwareList.Count} software items");
            return true;
        }
        catch (Exception ex)
        {
            _loggingService.LogError("Failed to load software list", ex);
            return false;
        }
    }

    /// <summary>
    /// Save software list to JSON file
    /// </summary>
    public async Task<bool> SaveSoftwareListAsync()
    {
        try
        {
            var directory = Path.GetDirectoryName(_dataPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(_softwareList, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_dataPath, json);

            _loggingService.LogInfo("Software list saved successfully");
            return true;
        }
        catch (Exception ex)
        {
            _loggingService.LogError("Failed to save software list", ex);
            return false;
        }
    }

    /// <summary>
    /// Get all software items
    /// </summary>
    public List<SoftwareItem> GetAllSoftware() => new(_softwareList);

    /// <summary>
    /// Get software by ID
    /// </summary>
    public SoftwareItem? GetSoftwareById(string id) => _softwareList.FirstOrDefault(s => s.Id == id);

    /// <summary>
    /// Search software by name or description
    /// </summary>
    public List<SoftwareItem> SearchSoftware(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new(_softwareList);

        var lowerQuery = query.ToLower();
        return _softwareList
            .Where(s => s.Name.ToLower().Contains(lowerQuery) ||
                       s.Description.ToLower().Contains(lowerQuery) ||
                       s.Publisher.ToLower().Contains(lowerQuery))
            .ToList();
    }

    /// <summary>
    /// Get software grouped by category
    /// </summary>
    public Dictionary<string, List<SoftwareItem>> GetGroupedByCategory()
    {
        return _softwareList
            .GroupBy(s => s.Category)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    /// <summary>
    /// Get list of unique categories
    /// </summary>
    public List<string> GetCategories() => _softwareList.Select(s => s.Category).Distinct().OrderBy(c => c).ToList();

    /// <summary>
    /// Get software by category
    /// </summary>
    public List<SoftwareItem> GetSoftwareByCategory(string category) =>
        _softwareList.Where(s => s.Category == category).ToList();

    /// <summary>
    /// Get selected software items
    /// </summary>
    public List<SoftwareItem> GetSelectedSoftware() =>
        _softwareList.Where(s => s.IsSelected).ToList();

    /// <summary>
    /// Get total size of selected software
    /// </summary>
    public long GetSelectedTotalSize() =>
        GetSelectedSoftware().Sum(s => s.Size);

    /// <summary>
    /// Update selection status of a software
    /// </summary>
    public void UpdateSelection(string id, bool isSelected)
    {
        var software = GetSoftwareById(id);
        if (software != null)
        {
            software.IsSelected = isSelected;
        }
    }

    /// <summary>
    /// Clear all selections
    /// </summary>
    public void ClearAllSelections()
    {
        foreach (var software in _softwareList)
        {
            software.IsSelected = false;
        }
    }

    /// <summary>
    /// Select all software
    /// </summary>
    public void SelectAll()
    {
        foreach (var software in _softwareList)
        {
            software.IsSelected = true;
        }
    }

    /// <summary>
    /// Add a new software item
    /// </summary>
    public void AddSoftware(SoftwareItem software)
    {
        if (!_softwareList.Any(s => s.Id == software.Id))
        {
            _softwareList.Add(software);
        }
    }

    /// <summary>
    /// Remove a software item
    /// </summary>
    public bool RemoveSoftware(string id)
    {
        var software = GetSoftwareById(id);
        if (software != null)
        {
            return _softwareList.Remove(software);
        }
        return false;
    }

    /// <summary>
    /// Create default software list
    /// </summary>
    private async Task CreateDefaultListAsync()
    {
        _softwareList = new List<SoftwareItem>
        {
            new() { Id = "1", Name = "Visual Studio Code", Publisher = "Microsoft", Version = "1.84.0", Category = "Development", Description = "Code editor", Size = 100 * 1024 * 1024, WingetId = "Microsoft.VisualStudioCode" },
            new() { Id = "2", Name = "Git", Publisher = "The Git Development Community", Version = "2.42.0", Category = "Development", Description = "Version control system", Size = 60 * 1024 * 1024, WingetId = "Git.Git" },
            new() { Id = "3", Name = ".NET SDK", Publisher = "Microsoft", Version = "8.0", Category = "Development", Description = ".NET development framework", Size = 600 * 1024 * 1024, WingetId = "Microsoft.DotNet.SDK.8" },
            new() { Id = "4", Name = "NodeJS", Publisher = "OpenJS Foundation", Version = "20.0.0", Category = "Development", Description = "JavaScript runtime", Size = 50 * 1024 * 1024, WingetId = "OpenJS.NodeJS" },
            new() { Id = "5", Name = "Python", Publisher = "Python Software Foundation", Version = "3.11.0", Category = "Development", Description = "Python programming language", Size = 100 * 1024 * 1024, WingetId = "Python.Python.3.11" },
            new() { Id = "6", Name = "7-Zip", Publisher = "Igor Pavlov", Version = "23.0", Category = "Utilities", Description = "Archive utility", Size = (long)(1.5 * 1024 * 1024), WingetId = "7zip.7zip" },
            new() { Id = "7", Name = "VLC Media Player", Publisher = "VideoLAN", Version = "3.0.0", Category = "Media", Description = "Media player", Size = 40 * 1024 * 1024, WingetId = "VideoLAN.VLC" },
            new() { Id = "8", Name = "Notepad++", Publisher = "Don Ho", Version = "8.5.0", Category = "Development", Description = "Text editor", Size = 7 * 1024 * 1024, WingetId = "Notepad++.Notepad++" },
            new() { Id = "9", Name = "Google Chrome", Publisher = "Google", Version = "117.0.0", Category = "Browsers", Description = "Web browser", Size = 150 * 1024 * 1024, WingetId = "Google.Chrome" },
            new() { Id = "10", Name = "Mozilla Firefox", Publisher = "Mozilla", Version = "117.0.0", Category = "Browsers", Description = "Web browser", Size = 80 * 1024 * 1024, WingetId = "Mozilla.Firefox" }
        };

        await SaveSoftwareListAsync();
    }
}
