using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IzSetup.Models;
using IzSetup.Services;

namespace IzSetup.ViewModels;

/// <summary>
/// View model for software selection screen
/// </summary>
public partial class SelectionScreenViewModel : ObservableObject
{
    private readonly SoftwareDataService _softwareDataService;
    private readonly LoggingService _loggingService;

    [ObservableProperty]
    private List<SoftwareItem> softwareList = [];

    [ObservableProperty]
    private List<string> categories = [];

    [ObservableProperty]
    private string selectedCategory = "All";

    [ObservableProperty]
    private string searchQuery = "";

    [ObservableProperty]
    private int selectedCount = 0;

    [ObservableProperty]
    private string selectedCountText = "0 selected";

    public List<SoftwareItem> SelectedSoftware => SoftwareList.Where(s => s.IsSelected).ToList();

    public SelectionScreenViewModel(SoftwareDataService softwareDataService, LoggingService loggingService)
    {
        _softwareDataService = softwareDataService;
        _loggingService = loggingService;
    }

    /// <summary>
    /// Load software list
    /// </summary>
    public async Task LoadSoftwareAsync()
    {
        SoftwareList = _softwareDataService.GetAllSoftware();
        var cats = _softwareDataService.GetCategories();
        Categories = new List<string> { "All" };
        Categories.AddRange(cats);
        SelectedCategory = "All";
        RefreshSoftwareList();
    }

    /// <summary>
    /// Refresh displayed software list based on filters
    /// </summary>
    [RelayCommand]
    public void RefreshSoftwareList()
    {
        var filtered = _softwareDataService.GetAllSoftware();

        // Apply category filter
        if (SelectedCategory != "All")
        {
            filtered = filtered.Where(s => s.Category == SelectedCategory).ToList();
        }

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var query = SearchQuery.ToLower();
            filtered = filtered
                .Where(s => s.Name.ToLower().Contains(query) ||
                           s.Description.ToLower().Contains(query) ||
                           s.Publisher.ToLower().Contains(query))
                .ToList();
        }

        SoftwareList = filtered;
        UpdateSelectedCount();
    }

    /// <summary>
    /// Toggle selection of a software
    /// </summary>
    [RelayCommand]
    public void ToggleSoftwareSelection(SoftwareItem? software)
    {
        if (software == null) return;

        software.IsSelected = !software.IsSelected;
        _loggingService.LogDebug($"{(software.IsSelected ? "Selected" : "Deselected")}: {software.Name}");
        UpdateSelectedCount();
    }

    /// <summary>
    /// Select all software
    /// </summary>
    [RelayCommand]
    public void SelectAll()
    {
        foreach (var software in SoftwareList)
        {
            software.IsSelected = true;
        }
        UpdateSelectedCount();
        _loggingService.LogInfo("Selected all software");
    }

    /// <summary>
    /// Deselect all software
    /// </summary>
    [RelayCommand]
    public void DeselectAll()
    {
        foreach (var software in SoftwareList)
        {
            software.IsSelected = false;
        }
        UpdateSelectedCount();
        _loggingService.LogInfo("Deselected all software");
    }

    /// <summary>
    /// Clear selections
    /// </summary>
    public void ClearSelections()
    {
        _softwareDataService.ClearAllSelections();
        foreach (var software in SoftwareList)
        {
            software.IsSelected = false;
        }
        UpdateSelectedCount();
    }

    /// <summary>
    /// Update selected count display
    /// </summary>
    private void UpdateSelectedCount()
    {
        SelectedCount = SelectedSoftware.Count;
        SelectedCountText = $"{SelectedCount} selected";
    }

    /// <summary>
    /// Handle category selection change
    /// </summary>
    partial void OnSelectedCategoryChanged(string value)
    {
        RefreshSoftwareListCommand.Execute(null);
    }

    /// <summary>
    /// Handle search query change
    /// </summary>
    partial void OnSearchQueryChanged(string value)
    {
        RefreshSoftwareListCommand.Execute(null);
    }
}
