using System.Windows;
using IzSetup.ViewModels;

namespace IzSetup;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new MainWindowViewModel();
    }
}
