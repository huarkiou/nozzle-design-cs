using Avalonia.Controls;
using GuiApp.ViewModels;

namespace GuiApp.Views;

public partial class OtnControl : UserControl
{
    public OtnControl()
    {
        InitializeComponent();
        EditCpSegments.Click += EditCpSegments_Click;
        EnableCpSegments.Click += EnableCpSegments_Click;
    }

    private async void EditCpSegments_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var vm = (OtnControlViewModel)DataContext!;
        var editor = new CpSegmentEditor(vm.CpSegments);
        await editor.ShowDialog<object>((Window)this.VisualRoot!);
    }

    private async void EnableCpSegments_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var vm = (OtnControlViewModel)DataContext!;
        vm.CpSegments.AddSegmentCommand.Execute(null);
        var editor = new CpSegmentEditor(vm.CpSegments);
        await editor.ShowDialog<object>((Window)this.VisualRoot!);
    }
}
