using Avalonia.Controls;
using GuiApp.ViewModels;

namespace GuiApp.Views;

public partial class SltnControl : UserControl
{
    public SltnControl()
    {
        InitializeComponent();
        DataContext = new SltnControlViewModel();
    }
}