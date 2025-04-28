using CommunityToolkit.Mvvm.ComponentModel;
using Corelib.Geometry;

namespace GuiApp.ViewModels;

public partial class CrossSectionSuperEllipseViewModel : ClosedCurveViewModel
{
    [ObservableProperty]
    public partial double X { get; set; } = 0;
    public static string XToolTip { get; set; } = "中心x坐标";
    [ObservableProperty]
    public partial double Y { get; set; } = 0.1;
    public static string YToolTip { get; set; } = "中心y坐标";
    [ObservableProperty]
    public partial double A { get; set; } = 0.6;
    public static string AToolTip { get; set; } = "长半轴长a";
    [ObservableProperty]
    public partial double B { get; set; } = 0.4;
    public static string BToolTip { get; set; } = "短半轴长b";
    [ObservableProperty]
    public partial double Alpha { get; set; } = 0;
    public static string AlphaToolTip { get; set; } = "旋转角α(长轴与θ=0轴的夹角)/°";
    [ObservableProperty]
    public partial double Power { get; set; } = 2;
    public static string PowerToolTip { get; set; } =
        "幂次n\n注意：0<n<2时，超椭圆也称为次椭圆，形状类似菱形；n=2时，超椭圆形状即椭圆；n>2时，称为过椭圆，形状为四角有圆角的矩形；n->+∞时，超椭圆形状即为矩形";


    public override string GetTomlString()
    {
        const string ret = """
                           # 是否用基准流场的进/出口截面高度对坐标进行标准化到单位圆内(若为false，则后续长度单位均为m，默认为true)
                           normalized = true
                           # 形状
                           shape = 'superellipse'
                           # 中心坐标(z,y)
                           center = [0, 0.25]
                           # 长半轴长z
                           a = 0.3
                           # 短半轴长y
                           b = 0.2
                           # 旋转角(长轴与θ=0轴的夹角)/°
                           alpha = 0
                           # 幂次(n<2时，超椭圆也称为次椭圆，形状类似菱形；n=2时，超椭圆形状即椭圆；n>2时，称为过椭圆，形状为四角有圆角的矩形；n->∞时，超椭圆形状即为矩形)
                           n = 2
                           """;
        var model = Tomlyn.Toml.ToModel(ret);
        model["normalized"] = IsNormalized;
        model["center"] = (double[]) [X, Y];
        model["a"] = A;
        model["b"] = B;
        model["alpha"] = Alpha;
        model["n"] = Power;
        return Tomlyn.Toml.FromModel(model);
    }

    public override IClosedCurve? GetClosedCurve()
    {
        return new SuperEllipse(X, Y, A, B, Power, double.DegreesToRadians(Alpha));
    }

    public override IClosedCurve? GetRawClosedCurve()
    {
        return IsNormalized
            ? new SuperEllipse(X * HNorm, Y * HNorm, A * HNorm, B * HNorm, Power, double.DegreesToRadians(Alpha))
            : GetClosedCurve();
    }

    public override IClosedCurve? GetNormalizedClosedCurve()
    {
        return IsNormalized
            ? GetClosedCurve()
            : new SuperEllipse(X / HNorm, Y / HNorm, A / HNorm, B / HNorm, Power, double.DegreesToRadians(Alpha));
    }

    protected override void OnNormalizedStateChanged()
    {
        if (IsNormalized)
        {
            X /= HNorm;
            Y /= HNorm;
            A /= HNorm;
            B /= HNorm;
        }
        else
        {
            X *= HNorm;
            Y *= HNorm;
            A *= HNorm;
            B *= HNorm;
        }
    }
}