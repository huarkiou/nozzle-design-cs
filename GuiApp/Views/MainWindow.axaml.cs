using Avalonia.Controls;
using Avalonia.Interactivity;
using GuiApp.ViewModels;

namespace GuiApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var vmOtn = new OtnControlViewModel();
        var vmSltn = new SltnControlViewModel();
        DataContext = new MainWindowViewModel(vmOtn, vmSltn);
        OtnControl.DataContext = vmOtn;
        SltnControl.DataContext = vmSltn;
    }

    private void Quit_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}