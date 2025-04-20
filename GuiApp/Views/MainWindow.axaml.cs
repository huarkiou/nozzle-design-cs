using Avalonia.Controls;
using Avalonia.Interactivity;
using GuiApp.ViewModels;

namespace GuiApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Quit(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}