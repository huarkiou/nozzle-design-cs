using Avalonia;
using Avalonia.Controls;

namespace GuiApp.Views;

public partial class LabeledInput : UserControl
{
    public LabeledInput()
    {
        InitializeComponent();
        TextBoxInput.TextChanged += (_, _) => { Input = TextBoxInput.Text; };
    }

    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<LabeledInput, string?>(nameof(Label));
    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly StyledProperty<string?> InputProperty =
        AvaloniaProperty.Register<LabeledInput, string?>(nameof(Input));

    public string? Input
    {
        get => GetValue(InputProperty);
        set => SetValue(InputProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == LabelProperty)
        {
            TextBlockLabel.Text = Label ?? string.Empty;
        }
        else if (change.Property == InputProperty)
        {
            TextBoxInput.Text = Input ?? string.Empty;
        }
    }
}