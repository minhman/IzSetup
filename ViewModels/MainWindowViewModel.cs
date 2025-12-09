using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IzSetup.Services;

namespace IzSetup.ViewModels;

/// <summary>
/// Main view model for application navigation and state management
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    private readonly SoftwareDataService _softwareDataService;
    private readonly SoftwareStatusService _softwareStatusService;
    private readonly SettingsService _settingsService;
    private readonly LoggingService _loggingService;
    private readonly LoadingScreenViewModel _loadingViewModel;
    private readonly SelectionScreenViewModel _selectionViewModel;
    private readonly ReviewScreenViewModel _reviewViewModel;
    private readonly InstallationScreenViewModel _installationViewModel;

    [ObservableProperty]
    private int currentScreenIndex = -1; // -1 = Loading, 0 = Selection, 1 = Review, 2 = Installation

    [ObservableProperty]
    private string windowTitle = "IzSetup - Bulk Software Installer";

    [ObservableProperty]
    private string screenTitle = "Checking Software Status";

    [ObservableProperty]
    private bool canGoBack = false;

    [ObservableProperty]
    private bool canGoForward = false;

    public LoadingScreenViewModel LoadingViewModel => _loadingViewModel;
    public SelectionScreenViewModel SelectionViewModel => _selectionViewModel;
    public ReviewScreenViewModel ReviewViewModel => _reviewViewModel;
    public InstallationScreenViewModel InstallationViewModel => _installationViewModel;

    public MainWindowViewModel()
    {
        _loggingService = new LoggingService();
        _softwareDataService = new SoftwareDataService(loggingService: _loggingService);
        _softwareStatusService = new SoftwareStatusService(_loggingService);
        _settingsService = new SettingsService(loggingService: _loggingService);
        
        _loadingViewModel = new LoadingScreenViewModel(_softwareDataService, _softwareStatusService, _loggingService);
        _selectionViewModel = new SelectionScreenViewModel(_softwareDataService, _softwareStatusService, _loggingService);
        _reviewViewModel = new ReviewScreenViewModel(_softwareDataService, _loggingService);
        _installationViewModel = new InstallationScreenViewModel(_softwareDataService, _settingsService, _loggingService);

        // Subscribe to check completion event
        _loadingViewModel.CheckCompleted += TransitionToSelection;

        InitializeAsync();
    }

    /// <summary>
    /// Transition from loading to selection screen
    /// </summary>
    private void TransitionToSelection()
    {
        CurrentScreenIndex = 0;
        ScreenTitle = "Select Software";
        CanGoBack = false;
        CanGoForward = true;
        _loggingService.LogInfo("Transitioned to Selection Screen");
    }

    /// <summary>
    /// Initialize view model with data and check software status
    /// </summary>
    private async void InitializeAsync()
    {
        try
        {
            // Show loading screen
            CurrentScreenIndex = -1;
            ScreenTitle = "Checking Software Status";
            CanGoBack = false;
            CanGoForward = false;

            // Load settings
            await _settingsService.LoadSettingsAsync();

            // Load and check software status (this will trigger TransitionToSelection via event)
            await _loadingViewModel.LoadAndCheckSoftwareAsync();

            // Initialize selection view with checked software
            await _selectionViewModel.LoadSoftwareAsync();

            _loggingService.LogInfo("Application initialized and ready");
        }
        catch (Exception ex)
        {
            _loggingService.LogError("Failed to initialize application", ex);
            ScreenTitle = "Initialization failed";
        }
    }

    /// <summary>
    /// Navigate to next screen
    /// </summary>
    [RelayCommand]
    public void GoForward()
    {
        if (CurrentScreenIndex == 0)
        {
            // Moving from Selection to Review
            if (_selectionViewModel.SelectedSoftware.Count == 0)
            {
                _loggingService.LogWarning("No software selected");
                return;
            }

            CurrentScreenIndex = 1;
            ScreenTitle = "Review & Confirm";
            CanGoBack = true;
            CanGoForward = true;
            _reviewViewModel.RefreshSelectedSoftware();
        }
        else if (CurrentScreenIndex == 1)
        {
            // Moving from Review to Installation
            CurrentScreenIndex = 2;
            ScreenTitle = "Installation Progress";
            CanGoBack = false;
            CanGoForward = false;
            _installationViewModel.StartInstallation(_selectionViewModel.SelectedSoftware);
        }
    }

    /// <summary>
    /// Navigate to previous screen
    /// </summary>
    [RelayCommand]
    public void GoBack()
    {
        if (CurrentScreenIndex > 0)
        {
            CurrentScreenIndex--;
            CanGoBack = CurrentScreenIndex > 0;
            CanGoForward = true;

            if (CurrentScreenIndex == 0)
            {
                ScreenTitle = "Select Software";
                _selectionViewModel.RefreshSoftwareList();
            }
            else if (CurrentScreenIndex == 1)
            {
                ScreenTitle = "Review & Confirm";
                _reviewViewModel.RefreshSelectedSoftware();
            }
        }
    }

    /// <summary>
    /// Finish installation and reset
    /// </summary>
    [RelayCommand]
    public void Finish()
    {
        _selectionViewModel.ClearSelections();
        _installationViewModel.Reset();
        CurrentScreenIndex = 0;
        ScreenTitle = "Select Software";
        CanGoBack = false;
        CanGoForward = true;
    }

    /// <summary>
    /// Quit application
    /// </summary>
    [RelayCommand]
    public void Quit()
    {
        _loggingService.LogInfo("Application closed");
    }
}
