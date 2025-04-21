using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace GuiApp.Views;

public partial class BinarySelector : UserControl
{
    public BinarySelector()
    {
        InitializeComponent();
        IsTrue = !IsTrue;
        IsTrue = !IsTrue;
        TrueRadioButton.IsCheckedChanged += (_, _) => { IsTrue = TrueRadioButton.IsChecked == true; };
    }

    public static readonly StyledProperty<string?> LabelTrueProperty =
        AvaloniaProperty.Register<BinarySelector, string?>(nameof(LabelTrue));
    public string? LabelTrue
    {
        get => GetValue(LabelTrueProperty);
        set => SetValue(LabelTrueProperty, value);
    }

    public static readonly StyledProperty<string?> LabelFalseProperty =
        AvaloniaProperty.Register<BinarySelector, string?>(nameof(LabelFalse));
    public string? LabelFalse
    {
        get => GetValue(LabelFalseProperty);
        set => SetValue(LabelFalseProperty, value);
    }

    public static readonly StyledProperty<bool> IsTrueProperty =
        AvaloniaProperty.Register<BinarySelector, bool>(nameof(IsTrue));

    public bool IsTrue
    {
        get => GetValue(IsTrueProperty);
        set => SetValue(IsTrueProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == LabelTrueProperty)
        {
            TrueRadioButton.Content = LabelTrue ?? string.Empty;
        }
        else if (change.Property == LabelFalseProperty)
        {
            FalseRadioButton.Content = LabelFalse ?? string.Empty;
        }
        else if (change.Property == IsTrueProperty)
        {
            TrueRadioButton.IsChecked ??= change.NewValue as bool?;
        }
    }
}