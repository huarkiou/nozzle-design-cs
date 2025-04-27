using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Corelib.Geometry;

namespace GuiApp.ViewModels;

public abstract partial class ClosedCurveViewModel : ViewModelBase
{
    public abstract IClosedCurve? GetClosedCurve();
    public abstract string GetTomlString();

    public abstract IClosedCurve? GetRawClosedCurve();
    public abstract IClosedCurve? GetNormalizedClosedCurve();
    protected abstract void OnNormalizedStateChanged();

    [ObservableProperty]
    public partial bool IsNormalized { get; set; } = true;
    public static string IsNormalizedToolTip => "是否对进/出口截面坐标分别用基准流场进/出口截面高度进行归一化";

    partial void OnIsNormalizedChanged(bool oldValue, bool newValue)
    {
        if (oldValue == newValue) return;


        OnNormalizedStateChanged();
    }

    [ObservableProperty]
    public partial double HNorm { get; set; } = double.NaN;
}