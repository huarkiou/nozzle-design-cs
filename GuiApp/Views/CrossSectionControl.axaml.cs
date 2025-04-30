using Avalonia;
using Avalonia.Controls;
using GuiApp.Models;
using GuiApp.ViewModels;

namespace GuiApp.Views;

public partial class CrossSectionControl : UserControl
{
    public CrossSectionControl() : this(CrossSectionPosition.Inlet)
    {
    }

    public CrossSectionControl(CrossSectionPosition type)
    {
        InitializeComponent();
        var vm = new CrossSectionControlViewModel
        {
            Position = type
        };
        DataContext = vm;
    }

    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<LabeledInput, string?>(nameof(Label));
    public string? Label
    {
        get => GetValue(LabelProperty);
        init => SetValue(LabelProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == LabelProperty)
        {
            TextBlockLabel.Text = Label ?? string.Empty;
        }
    }
}