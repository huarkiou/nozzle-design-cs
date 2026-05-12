using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.DependencyInjection;
using GuiApp.ViewModels;

namespace GuiApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var vmOtn = Ioc.Default.GetRequiredService<OtnControlViewModel>();
        var vmSltn = Ioc.Default.GetRequiredService<SltnControlViewModel>();
        DataContext = Ioc.Default.GetRequiredService<MainWindowViewModel>();
        OtnControl.DataContext = vmOtn;
        SltnControl.DataContext = vmSltn;
    }

    private void Quit_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}