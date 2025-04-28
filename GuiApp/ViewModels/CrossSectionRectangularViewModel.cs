using CommunityToolkit.Mvvm.ComponentModel;
using Corelib.Geometry;

namespace GuiApp.ViewModels;

public partial class CrossSectionRectangularViewModel : ClosedCurveViewModel
{
    [ObservableProperty]
    public partial double X { get; set; } = 0;
    public static string XToolTip { get; set; } = "中心x坐标";
    [ObservableProperty]
    public partial double Y { get; set; } = 0.1;
    public static string YToolTip { get; set; } = "中心y坐标";
    [ObservableProperty]
    public partial double Length { get; set; } = 0.6;
    public static string LengthToolTip { get; set; } = "长度L";
    [ObservableProperty]
    public partial double Width { get; set; } = 0.4;
    public static string WidthToolTip { get; set; } = "宽度W";
    [ObservableProperty]
    public partial double Alpha { get; set; } = 0;
    public static string AlphaToolTip { get; set; } = "旋转角α";


    public override string GetTomlString()
    {
        const string ret = """
                           # 是否用基准流场的进/出口截面高度对坐标进行标准化到单位圆内(若为false，则后续长度单位均为m，默认为true)
                           normalized = true
                           # 形状
                           shape = 'rectangular'
                           # 中心坐标(z,y)
                           center = [0, 0.2]
                           # 长z
                           length = 0.8
                           # 宽y
                           width = 0.6
                           # 旋转角(长与θ=0轴的夹角)/°
                           alpha = 0
                           """;
        var model = Tomlyn.Toml.ToModel(ret);
        model["normalized"] = IsNormalized;
        model["center"] = (double[]) [X, Y];
        model["length"] = Length;
        model["width"] = Width;
        model["alpha"] = Alpha;
        return Tomlyn.Toml.FromModel(model);
    }

    public override IClosedCurve? GetClosedCurve()
    {
        return new Rectangular(X, Y, Length, Width, double.DegreesToRadians(Alpha));
    }

    public override IClosedCurve? GetRawClosedCurve()
    {
        return IsNormalized
            ? new Rectangular(X * HNorm, Y * HNorm, Length * HNorm, Width * HNorm, double.DegreesToRadians(Alpha))
            : GetClosedCurve();
    }

    public override IClosedCurve? GetNormalizedClosedCurve()
    {
        return IsNormalized
            ? GetClosedCurve()
            : new Rectangular(X / HNorm, Y / HNorm, Length / HNorm, Width / HNorm, double.DegreesToRadians(Alpha));
    }

    protected override void OnNormalizedStateChanged()
    {
        if (IsNormalized)
        {
            X /= HNorm;
            Y /= HNorm;
            Width /= HNorm;
            Length /= HNorm;
        }
        else
        {
            X *= HNorm;
            Y *= HNorm;
            Width *= HNorm;
            Length *= HNorm;
        }
    }
}