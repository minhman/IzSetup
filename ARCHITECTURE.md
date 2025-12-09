# IzSetup Architecture Overview

## High-Level Architecture

```
┌─────────────────────────────────────────────┐
│              WPF Frontend                   │
│  (Views: Selection, Review, Installation)   │
├─────────────────────────────────────────────┤
│         MVVM ViewModels & Commands          │
│  (MainWindowVM, SelectionVM, ReviewVM, etc) │
├─────────────────────────────────────────────┤
│         Business Logic Services             │
│  (Winget, Data, Settings, Logging)          │
├─────────────────────────────────────────────┤
│            Data Models                      │
│  (SoftwareItem, InstallationProgress, etc)  │
├─────────────────────────────────────────────┤
│         Windows Package Manager             │
│              (winget)                       │
└─────────────────────────────────────────────┘
```

## Component Relationships

### Initialization Flow

```
App.xaml.cs
  └─> MainWindow()
      └─> MainWindow.xaml.cs
          └─> InitializeApplication()
              ├─> Create SoftwareDataService
              ├─> Create SettingsService
              ├─> Create LoggingService
              ├─> Create WingetService
              ├─> Create MainWindowViewModel
              └─> Initialize & Load Software
```

### Navigation Flow

```
SelectionScreen
    ↓ (Next button)
    ↓ (User selects software)
ReviewScreen
    ↓ (Next button)
    ↓ (User agrees to terms)
InstallationScreen
    ↓ (Installation completes)
    ↓ (Finish button)
Back to SelectionScreen or Exit
```

### Data Flow Diagram

```
┌────────────────────────┐
│  software_list.json    │
└───────────┬────────────┘
            │
            ↓
┌────────────────────────────────────┐
│  SoftwareDataService               │
│  - LoadSoftwareListAsync()         │
│  - SearchSoftware()                │
│  - UpdateSoftwareSelection()       │
└───────────┬─────────────┬──────────┘
            │             │
    ┌───────┘       ┌─────┘
    ↓               ↓
SelectionScreenVM  ReviewScreenVM
    │               │
    └───────────────┴─────────────────┐
                                      ↓
                            MainWindowViewModel
                                      │
                                      ↓
                            InstallationScreenVM
                                      │
                            ┌─────────┴──────────┐
                            ↓                    ↓
                    WingetService         LoggingService
                            │                    │
                            ↓                    ↓
                        winget.exe        Log Files
```

## Service Interactions

### WingetService & InstallationScreenViewModel

```csharp
// Start installation
var selectedApps = _dataService.GetSelectedSoftware();

var progressReporter = new Progress<InstallationProgress>(progress =>
{
    UpdateProgressDisplay(progress);
});

var results = await _wingetService.InstallMultipleAsync(
    selectedApps.Select(s => s.Id),
    progressReporter,
    cancellationToken);
```

### LoggingService & UI

```csharp
// LoggingService events
_loggingService.LogAdded += OnLogAdded;

private void OnLogAdded(object? sender, LogEntry entry)
{
    InstallationLogs.Add(entry);  // Adds to ObservableCollection
}
```

### SoftwareDataService & ViewModels

```csharp
// Selection Screen
var softwareList = await _dataService.LoadSoftwareListAsync();

// User changes selection
_dataService.UpdateSoftwareSelection(appId, isSelected);

// Review Screen
var selected = _dataService.GetSelectedSoftware();
```

## State Management

### ObservableProperty Pattern

```csharp
[ObservableProperty]
private int selectedCount = 0;

// Automatically generates:
// - SelectedCount property
// - OnSelectedCountChanged() partial method
// - PropertyChanged notification
```

### RelayCommand Pattern

```csharp
[RelayCommand]
public void SelectAll()
{
    // ...
}

// Automatically generates:
// - SelectAllCommand IRelayCommand property
// - CanExecute checking (if method exists)
```

## JSON Data Structure

### software_list.json

```json
[
  {
    "id": "Google.Chrome",
    "name": "Google Chrome",
    "publisher": "Google",
    "description": "Fast web browser",
    "version": "131.0.6778",
    "category": "Browsers",
    "estimatedSizeMB": 150,
    "homepage": "https://www.google.com/chrome/",
    "iconUrl": "",
    "installerUrl": null,
    "notes": "Latest version",
    "lastUpdated": "2025-12-09T00:00:00",
    "isSelected": false
  }
]
```

### appsettings.json

```json
{
  "dataDirectory": "C:\\Users\\{username}\\AppData\\Roaming\\IzSetup",
  "softwareListPath": "Data/software_list.json",
  "settingsFilePath": "appsettings.json",
  "wingetPath": "winget",
  "allowParallelInstallation": false,
  "language": "vi-VN",
  "useDarkMode": true,
  "autoStartInstallation": false,
  "enableDetailedLogging": true,
  "saveInstallationHistory": true,
  "keepHistoryDays": 30,
  "wingetTimeoutMs": 300000,
  "checkForUpdates": true,
  "showNotificationOnCompletion": true
}
```

## Winget Integration

### Installation Command

```bash
winget install --id {appId} --silent --accept-source-agreements --accept-package-agreements
```

### Process Flow

```csharp
public async Task<bool> InstallAsync(
    string appId,
    IProgress<InstallationProgress> progress,
    CancellationToken cancellationToken)
{
    // 1. Create ProcessStartInfo
    // 2. Start process with redirected streams
    // 3. Wait for exit with timeout
    // 4. Read output and error
    // 5. Report progress
    // 6. Handle errors
    // 7. Return success/failure
}
```

### Error Handling

- Exit code 0 = Success
- Exit code != 0 = Failure
- Timeout after 5 minutes
- Cancellation via CancellationToken
- Output captured to logs

## Extension Points

### Adding New Package Manager

Create new service:
```csharp
public interface IPackageManager
{
    Task<bool> InstallAsync(string appId, IProgress<InstallationProgress> progress);
}

public class ChocolateyService : IPackageManager
{
    // Implementation
}
```

### Adding New Data Source

Create data service wrapper:
```csharp
public class CloudSoftwareDataService
{
    public async Task<List<SoftwareItem>> LoadFromCloudAsync(string url)
    {
        // Load from remote API
    }
}
```

### Adding New UI Screens

1. Create XAML page in `Views/`
2. Create ViewModel in `ViewModels/`
3. Add to `MainWindowViewModel`
4. Implement navigation logic

## Performance Considerations

### Current Optimizations
- Async/await for non-blocking operations
- Log limit (1000 entries) to prevent memory issues
- Single app installation at a time (sequential)
- Cancellation support via CancellationToken

### Potential Improvements
- Lazy loading for large software lists
- Pagination for software list display
- Background task for checking updates
- Caching for software metadata
- Parallel installation (with caution)

## Security Considerations

### Current Implementation
- WPF application sandboxing
- No admin elevation required (winget handles it)
- Settings stored in user AppData (non-privileged)
- JSON files not encrypted (user assumption)

### Future Enhancements
- Digital signature verification for software list
- HTTPS for remote software list fetching
- Encrypted settings storage
- Malware detection integration

---

This architecture is designed to be:
- **Modular**: Services can be swapped/extended
- **Testable**: MVVM pattern facilitates unit testing
- **Maintainable**: Clear separation of concerns
- **Scalable**: Easy to add new features

