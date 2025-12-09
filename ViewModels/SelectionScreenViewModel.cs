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
    private readonly SoftwareStatusService _softwareStatusService;
    private readonly LoggingService _loggingService;
    private List<SoftwareItem> _allSoftware = [];

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

    [ObservableProperty]
    private string selectedTab = "Install"; // Install, Installed, Update

    public List<SoftwareItem> SelectedSoftware => SoftwareList.Where(s => s.IsSelected).ToList();

    public SelectionScreenViewModel(SoftwareDataService softwareDataService, SoftwareStatusService softwareStatusService, LoggingService loggingService)
    {
        _softwareDataService = softwareDataService;
        _softwareStatusService = softwareStatusService;
        _loggingService = loggingService;
    }

    /// <summary>
    /// Load software list and check installation status
    /// </summary>
    public async Task LoadSoftwareAsync()
    {
        try
        {
            await _softwareDataService.LoadSoftwareListAsync();
            _allSoftware = new List<SoftwareItem>(_softwareDataService.GetAllSoftware());
            
            // Check installation status for all software
            await _softwareStatusService.CheckAndUpdateStatusAsync(_allSoftware);

            var cats = _softwareDataService.GetCategories();
            Categories = new List<string> { "All" };
            Categories.AddRange(cats);
            SelectedCategory = "All";
            SelectedTab = "Install";
            
            RefreshSoftwareListCommand.Execute(null);
        }
        catch (Exception ex)
        {
            _loggingService.LogError("Failed to load software", ex);
        }
    }

    /// <summary>
    /// Refresh displayed software list based on filters and tab
    /// </summary>
    [RelayCommand]
    public void RefreshSoftwareList()
    {
        // Preserve current selections
        var selectedIds = SoftwareList.Where(s => s.IsSelected).Select(s => s.WingetId).ToHashSet();
        
        var filtered = _allSoftware.ToList();

        // Filter by tab
        if (SelectedTab == "Install")
        {
            filtered = filtered.Where(s => s.InstallationStatus == "NotInstalled").ToList();
        }
        else if (SelectedTab == "Installed")
        {
            filtered = filtered.Where(s => s.InstallationStatus == "Installed").ToList();
        }
        else if (SelectedTab == "Update")
        {
            filtered = filtered.Where(s => s.InstallationStatus == "UpdateAvailable").ToList();
        }

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

        // Restore selection state
        foreach (var item in filtered)
        {
            item.IsSelected = selectedIds.Contains(item.WingetId);
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

    /// <summary>
    /// Select a tab
    /// </summary>
    [RelayCommand]
    public void SelectTab(string? tabName)
    {
        if (!string.IsNullOrEmpty(tabName))
        {
            SelectedTab = tabName;
        }
    }

    /// <summary>
    /// Handle tab change
    /// </summary>
    partial void OnSelectedTabChanged(string value)
    {
        RefreshSoftwareListCommand.Execute(null);
    }
}

