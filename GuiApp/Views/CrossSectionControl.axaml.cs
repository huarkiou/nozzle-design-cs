using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using GuiApp.ViewModels;

namespace GuiApp.Views;

public partial class CrossSectionControl : UserControl
{
    public CrossSectionControl()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<LabeledInput, string?>(nameof(Label));
    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
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