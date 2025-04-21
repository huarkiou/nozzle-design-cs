using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GuiApp.Views;

public partial class LabeledSlider : UserControl
{
    public LabeledSlider()
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

    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<LabeledInput, double>(nameof(Value));

    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly StyledProperty<double> MinimumProperty =
        AvaloniaProperty.Register<LabeledInput, double>(nameof(Minimum));
    public double Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<LabeledInput, double>(nameof(Maximum));
    public double Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == LabelProperty)
        {
            TextBlockLabel.Text = Label ?? string.Empty;
        }
        else if (change.Property == ValueProperty)
        {
            SliderValue.Value = Value;
        }
        else if (change.Property == MinimumProperty)
        {
            SliderValue.Minimum = Minimum;
        }
        else if (change.Property == MaximumProperty)
        {
            SliderValue.Maximum = Maximum;
        }
    }
}