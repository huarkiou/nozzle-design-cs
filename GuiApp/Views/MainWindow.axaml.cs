using Avalonia.Controls;
using Avalonia.Interactivity;
using GuiApp.ViewModels;

namespace GuiApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        OtnControl.DataContext = new OtnControlViewModel();
        // SltnControl.DataContext = new SltnControlViewModel();
    }

    private void Quit(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}