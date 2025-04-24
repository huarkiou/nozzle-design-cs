using CommunityToolkit.Mvvm.ComponentModel;
using Corelib.Geometry;

namespace GuiApp.ViewModels;

public partial class CrossSectionCircleViewModel : ViewModelBase, IClosedCurveViewModel
{
    [ObservableProperty]
    public partial double X { get; set; } = 0;
    [ObservableProperty]
    public partial double Y { get; set; } = 0.1;
    [ObservableProperty]
    public partial double Radius { get; set; } = 0.4;

    public IClosedCurve? GetClosedCurve()
    {
        return new Circle(X, Y, Radius);
    }

    public string GetTomlString()
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
        model["radius"] = Radius;
        model["center"] = (double[]) [X, Y];
        return Tomlyn.Toml.FromModel(model);
    }
}