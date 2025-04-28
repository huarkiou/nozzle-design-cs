using CommunityToolkit.Mvvm.ComponentModel;
using Corelib.Geometry;

namespace GuiApp.ViewModels;

public partial class CrossSectionCircleViewModel : ClosedCurveViewModel
{
    [ObservableProperty]
    public partial double X { get; set; } = 0;
    public static string XToolTip { get; set; } = "圆心x坐标";
    [ObservableProperty]
    public partial double Y { get; set; } = 0.1;
    public static string YToolTip { get; set; } = "圆心y坐标";
    [ObservableProperty]
    public partial double Radius { get; set; } = 0.4;
    public static string RadiusToolTip { get; set; } = "圆的半径R";


    public override string GetTomlString()
    {
        const string ret = """
                           # 是否用基准流场的进/出口截面高度对坐标进行标准化到单位圆内(若为false，则后续长度单位均为m，默认为true)
                           normalized = true
                           # 形状
                           shape = 'circle'
                           # 圆心坐标(z,y)
                           center = [0, 0.2]
                           # 圆的半径
                           radius = 0.8
                           """;
        var model = Tomlyn.Toml.ToModel(ret);
        model["normalized"] = IsNormalized;
        model["center"] = (double[]) [X, Y];
        model["radius"] = Radius;
        return Tomlyn.Toml.FromModel(model);
    }

    public override IClosedCurve? GetClosedCurve()
    {
        return new Circle(X, Y, Radius);
    }

    public override IClosedCurve? GetRawClosedCurve()
    {
        return IsNormalized ? new Circle(X * HNorm, Y * HNorm, Radius * HNorm) : GetClosedCurve();
    }

    public override IClosedCurve? GetNormalizedClosedCurve()
    {
        return IsNormalized ? GetClosedCurve() : new Circle(X / HNorm, Y / HNorm, Radius / HNorm);
    }

    protected override void OnNormalizedStateChanged()
    {
        if (IsNormalized)
        {
            X /= HNorm;
            Y /= HNorm;
            Radius /= HNorm;
        }
        else
        {
            X *= HNorm;
            Y *= HNorm;
            Radius *= HNorm;
        }
    }
}