# IzSetup - Bulk Software Installation Tool

**IzSetup** (Easy Setup) is a free, lightweight Windows desktop application for batch installing multiple software packages without bloatware. It uses Windows Package Manager (winget) as the backend for silent installations.

## Features

- ✅ **Easy Selection** - Browse and select multiple applications from an organized list
- ✅ **Review & Confirm** - Review your selections before installation
- ✅ **Real-time Monitoring** - Watch installation progress with detailed logs
- ✅ **Winget Integration** - Uses Windows Package Manager for clean installations
- ✅ **Customizable** - JSON-based software list and application settings
- ✅ **No Bloatware** - Direct installation without unwanted bundled software
- ✅ **Modern UI** - Clean, intuitive WPF interface

## Project Structure

```
IzSetup/
├── Models/
│   ├── SoftwareItem.cs           # Software item model
│   ├── InstallationModels.cs     # Installation progress & logs
│   └── AppSettings.cs             # Application configuration
├── Services/
│   ├── WingetService.cs           # Winget command execution
│   ├── SoftwareDataService.cs     # Software list management (JSON)
│   ├── SettingsService.cs         # Settings management
│   └── LoggingService.cs          # Application logging
├── ViewModels/
│   ├── MainWindowViewModel.cs     # Main view model (navigation)
│   ├── SelectionScreenViewModel.cs # Screen 1: Software selection
│   ├── ReviewScreenViewModel.cs   # Screen 2: Review & confirm
│   └── InstallationScreenViewModel.cs # Screen 3: Installation progress
├── Views/
│   ├── MainWindow.xaml(.cs)       # Main window
│   ├── SelectionScreen.xaml(.cs)  # Selection screen
│   ├── ReviewScreen.xaml(.cs)     # Review screen
│   └── InstallationScreen.xaml(.cs) # Installation screen
├── Data/
│   └── software_list.json         # Software database
├── Resources/
│   └── (Icons, images, etc.)
├── App.xaml(.cs)                  # Application entry point
├── appsettings.json               # Application settings
└── IzSetup.csproj                 # Project file
```

## Tech Stack

- **Framework**: .NET 8.0
- **UI**: WPF (Windows Presentation Foundation)
- **Architecture**: MVVM (Community Toolkit)
- **Backend**: Windows Package Manager (winget)
- **Data**: JSON (System.Text.Json)
- **Logging**: Custom file-based logging

## Requirements

- Windows 10 Build 19041 or later
- Windows Package Manager (winget) installed
- .NET 8.0 Runtime
- Visual Studio 2022 or later (for development)

## Installation

### Prerequisites
1. Ensure [winget](https://github.com/microsoft/winget-cli) is installed
2. Install [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
3. Visual Studio 2022 with .NET workload (optional, for development)

### Build & Run

```bash
# Clone repository
git clone <repository-url>
cd IzSetup

# Restore dependencies
dotnet restore

# Build
dotnet build

# Run
dotnet run
```

## Usage

### 1. Select Software
- Browse the organized list of applications by category
- Use the search bar to find specific software
- Click checkboxes to select applications
- Click "Select All" or "Deselect All" for quick actions

### 2. Review & Confirm
- Review your selections with detailed information
- See estimated download size and time
- Read and agree to terms of use
- Click "Install" to proceed

### 3. Installation
- Monitor real-time progress for each application
- View detailed installation logs
- See overall completion percentage
- Cancel installation if needed
- Click "Finish" when complete

## Configuration

### software_list.json

Define available software in `Data/software_list.json`:

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
    "notes": "Latest version"
  }
]
```

### appsettings.json

Configure application behavior:

```json
{
  "language": "vi-VN",
  "useDarkMode": true,
  "wingetTimeoutMs": 300000,
  "enableDetailedLogging": true
}
```

## Architecture

### MVVM Pattern
- **Models**: Data structures (SoftwareItem, InstallationProgress, etc.)
- **ViewModels**: Logic for each screen (using Community Toolkit MVVM)
- **Views**: XAML UI with code-behind

### Services
- **WingetService**: Manages winget command execution
- **SoftwareDataService**: Loads/saves software list from/to JSON
- **SettingsService**: Manages application configuration
- **LoggingService**: Handles application logging

### Navigation Flow
```
Selection Screen → Review Screen → Installation Screen → Completion
```

## Key Features Implementation

### 1. Winget Integration
- Executes `winget install --id {appId} --silent` commands
- Supports async/await with cancellation tokens
- Provides real-time progress updates
- Handles timeouts and errors gracefully

### 2. Software List Management
- Loads from JSON file on startup
- Creates default list if file doesn't exist
- Groups applications by category
- Supports search/filter functionality

### 3. Installation Monitoring
- Real-time progress bars
- Detailed installation logs
- Speed and ETA calculations
- Per-app and overall progress tracking

### 4. Settings Management
- Persistent JSON-based configuration
- Dark mode support
- Language preferences (ready for localization)
- Customizable timeouts and behaviors

## Future Enhancements

- [ ] Multi-language support (Vietnamese, English, Chinese, etc.)
- [ ] Installation history & rollback
- [ ] Custom installation profiles/bundles
- [ ] Advanced filtering and sorting
- [ ] System tray integration
- [ ] Auto-update checking
- [ ] Integration with other package managers (Chocolatey, etc.)
- [ ] Portable version

## Troubleshooting

### Winget not found
- Ensure Windows Package Manager is installed
- Check `wingetPath` in appsettings.json
- Try running `winget --version` in PowerShell

### Installation hangs
- Check winget logs: `%LOCALAPPDATA%\Packages\Microsoft.DesktopAppInstaller_*\LocalState\logs`
- Increase `wingetTimeoutMs` in appsettings.json
- Try installing individually in PowerShell: `winget install --id {appId}`

### Data file not found
- Software list file will be created automatically with default values
- Check `Data/software_list.json` path in settings

## Contributing

Contributions are welcome! Please follow these guidelines:
1. Fork the repository
2. Create a feature branch
3. Commit changes with descriptive messages
4. Submit a pull request

## License

MIT License - See LICENSE file for details

## Support

For issues, questions, or suggestions, please open an issue on GitHub.

---

**IzSetup** - Making software installation simple and clean! 🚀
