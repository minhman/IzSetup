# IzSetup Development Guide

## Quick Start

### 1. Prerequisites
- Visual Studio 2022 or JetBrains Rider
- .NET 8.0 SDK
- Git

### 2. Setup

```bash
# Clone and open
git clone <repository>
cd IzSetup

# Restore packages
dotnet restore

# Open in Visual Studio
# or build from command line
dotnet build

# Run
dotnet run
```

## Project Organization

### Models (`Models/`)
- **SoftwareItem.cs**: Software entry model
  - Contains: Id, Name, Publisher, Version, Category, Size, Homepage, etc.
  
- **InstallationModels.cs**: Installation-related models
  - `InstallationProgress`: Tracks progress of a single app installation
  - `InstallationStatus`: Enum for installation states
  - `LogEntry`: Log message with timestamp and level

- **AppSettings.cs**: Application configuration model
  - Contains all configurable settings
  - Loaded from appsettings.json

### Services (`Services/`)
- **WingetService.cs**: Windows Package Manager integration
  - `InstallAsync()`: Install single app
  - `InstallMultipleAsync()`: Install multiple apps sequentially
  - `IsWingetInstalledAsync()`: Check if winget is available
  - Handles async execution, cancellation, timeouts

- **SoftwareDataService.cs**: Software list management
  - `LoadSoftwareListAsync()`: Load from JSON
  - `SaveSoftwareListAsync()`: Save to JSON
  - `GetSoftwareByCategory()`: Get categorized list
  - `SearchSoftware()`: Search by name/description
  - `UpdateSoftwareSelection()`: Update selection state

- **SettingsService.cs**: Configuration management
  - `LoadSettingsAsync()`: Load from appsettings.json
  - `SaveSettingsAsync()`: Save to appsettings.json
  - `ResetToDefaultAsync()`: Reset to defaults

- **LoggingService.cs**: Application logging
  - `AddLog()`: Add log entry
  - `GetLogs()`: Get all logs
  - `GetLogsForApp()`: Get app-specific logs
  - Saves to file daily

### ViewModels (`ViewModels/`)
Uses Community Toolkit MVVM with:
- `[ObservableProperty]`: Auto-property change notification
- `[RelayCommand]`: Auto-command generation

**MainWindowViewModel.cs**
- Central view model for navigation
- Manages screen transitions (0=Selection, 1=Review, 2=Installation)
- Coordinates between other view models

**SelectionScreenViewModel.cs**
- Lists software with categories
- Search/filter functionality
- Selection state management
- "Select All" / "Deselect All" commands

**ReviewScreenViewModel.cs**
- Displays selected software summary
- Shows estimated size and time
- Manages terms agreement checkbox
- "Back" and "Proceed" navigation

**InstallationScreenViewModel.cs**
- Real-time progress tracking
- Overall and per-app progress bars
- Installation logs collection
- "Cancel" and "Finish" commands

### Views (`Views/`)
WPF XAML pages:

**MainWindow.xaml**
- Main application window
- Header with title/screen info
- Content frame for page navigation
- Footer with navigation buttons

**SelectionScreen.xaml**
- Software list with search
- Checkboxes for selection
- Category grouping
- "Select All" / "Deselect All" buttons

**ReviewScreen.xaml**
- Summary cards for selected software
- Installation summary (apps, size, time)
- Terms agreement checkbox
- "Back" and "Install" buttons

**InstallationScreen.xaml**
- Overall progress bar
- Individual app progress cards
- Installation log viewer
- "Cancel" and "Finish" buttons

### Data & Configuration
- **Data/software_list.json**: Software database (auto-created if missing)
- **appsettings.json**: Application settings
- **Logs/**: Installation logs (created per day)

### Helpers (`Helpers/`)
Utility classes:
- **TimeHelper**: Time formatting (2h 30m 45s)
- **FileSizeHelper**: Size formatting (150 MB)
- **ValidationHelper**: Input validation
- **StringHelper**: String utilities

## Data Flow

### Software Loading
```
App Start
  ↓
SettingsService.LoadSettingsAsync()
  ↓
SoftwareDataService.LoadSoftwareListAsync()
  ↓
Creates default list if missing
  ↓
SelectionScreenViewModel.LoadSoftwareAsync()
  ↓
Displays software in UI
```

### Selection Flow
```
User selects software
  ↓
SoftwareItem.IsSelected = true
  ↓
SoftwareDataService.UpdateSoftwareSelection()
  ↓
SelectionScreenViewModel.UpdateSelectedCount()
  ↓
UI updates "Selected: X" counter
```

### Installation Flow
```
User clicks "Install"
  ↓
ReviewScreenViewModel validates terms
  ↓
MainWindowViewModel.GoToNextScreenAsync()
  ↓
InstallationScreenViewModel.StartInstallationAsync()
  ↓
WingetService.InstallMultipleAsync()
  ↓
For each app: WingetService.InstallAsync()
  ↓
Reports progress via IProgress<InstallationProgress>
  ↓
UI updates progress bars and logs
  ↓
Installation complete
```

## Adding New Software

### Method 1: Edit JSON
```json
{
  "id": "Company.AppName",
  "name": "Application Name",
  "publisher": "Company",
  "description": "Brief description",
  "version": "1.0.0",
  "category": "Development",
  "estimatedSizeMB": 250,
  "homepage": "https://...",
  "notes": "Optional notes"
}
```

### Method 2: Programmatic
```csharp
var item = new SoftwareItem
{
    Id = "Company.App",
    Name = "Application",
    Publisher = "Company",
    Category = "Category",
    // ... other properties
};
dataService.AddSoftware(item);
await dataService.SaveSoftwareListAsync(/* current list */);
```

## Customizing Settings

### appsettings.json
```json
{
  "language": "vi-VN",
  "useDarkMode": true,
  "wingetTimeoutMs": 300000,
  "enableDetailedLogging": true,
  "wingetPath": "winget"
}
```

### Runtime Settings
```csharp
var settings = await settingsService.LoadSettingsAsync();
settings.UseDarkMode = false;
await settingsService.SaveSettingsAsync(settings);
```

## MVVM Pattern Notes

### ObservableProperty
```csharp
[ObservableProperty]
private string myProperty = "initial";
// Generates MyProperty property with OnPropertyChanged()
```

### RelayCommand
```csharp
[RelayCommand]
public void MyCommand()
{
    // Auto-generates MyCommandCommand property
}
```

### Two-Way Binding
```xml
<CheckBox IsChecked="{Binding TermsAgreed, Mode=TwoWay}" />
```

## Testing Locally

### Test Selection
1. Run app
2. Check/uncheck software
3. Verify "Selected: X" counter updates

### Test Review
1. Select some software
2. Click "Next"
3. Verify summary shows correct count and size
4. Try clicking "Install" without checking terms (should be disabled)

### Test Installation
1. Select 1-2 software
2. Agree to terms
3. Click "Install"
4. Monitor progress bars and logs
5. Verify each app installation status

### Test Without Winget
1. Change `wingetPath` in appsettings.json to invalid path
2. App should warn about missing winget on startup
3. Installation should fail gracefully

## Common Tasks

### Adding New UI Screen
1. Create XAML file in `Views/`
2. Create ViewModel in `ViewModels/`
3. Add ViewModel to MainWindowViewModel
4. Update MainWindowViewModel.CurrentScreenIndex logic
5. Add navigation button handlers

### Adding New Setting
1. Add property to AppSettings model
2. Add to appsettings.json
3. Create UI control for setting (optional)
4. Load/save via SettingsService

### Adding Logging
```csharp
_loggingService.AddLog(
    "Message text",
    LogLevel.Info,
    "optional-app-id");
```

### Adding Search Filter
```csharp
var results = _dataService.SearchSoftware(query);
// Returns filtered list by name/description/publisher
```

## Debugging Tips

### Enable Detailed Logging
- Set `enableDetailedLogging: true` in appsettings.json
- Logs saved to `AppData/Roaming/IzSetup/Logs/izsetup_YYYY-MM-DD.log`

### Check Winget
```powershell
winget --version
winget list
winget search Google.Chrome
```

### Async/Await Issues
- Always use `await` for async methods
- Use CancellationToken for cancellable operations
- Monitor for deadlocks in UI thread

### XAML Binding Issues
- Enable diagnostic output in debug output window
- Check DataContext binding (usually set in code-behind)
- Verify property names match exactly

## Build & Deploy

### Debug Build
```bash
dotnet build --configuration Debug
```

### Release Build
```bash
dotnet publish --configuration Release --self-contained
```

### MSIX Packaging
Requires Windows App SDK build tools:
```bash
dotnet publish -c Release /p:WindowsPackageType=MSIX
```

## Performance Optimization

### Large Software Lists
- Consider pagination instead of showing all at once
- Use virtualization for ListBox/ItemsControl

### Installation Progress
- Winget parsing can be slow; consider background thread
- Throttle log updates to prevent UI freeze

### Memory Usage
- Limit log entries (currently 1000 max)
- Dispose CancellationTokenSource when done

## Next Steps

1. **Localization**: Implement multi-language support
2. **Themes**: Add light/dark theme support
3. **Advanced Filtering**: Add category, size, popularity filters
4. **Installation Profiles**: Save/load installation bundles
5. **Uninstall**: Add uninstall support
6. **Update Check**: Verify installed versions vs latest

---

Happy coding! 🚀
