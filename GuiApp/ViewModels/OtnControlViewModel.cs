namespace GuiApp.ViewModels;

public partial class OtnControlViewModel : ViewModelBase
{
    public bool IsAxisymmetric { get; set; } = true;
    public double Width { get; } = 1.0;
    public double InletHeight { get; } = 1.0;
}