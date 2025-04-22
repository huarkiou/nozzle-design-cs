using CommunityToolkit.Mvvm.ComponentModel;
using Corelib.Geometry;

namespace GuiApp.ViewModels;

public partial class CrossSectionEllipseViewModel : ViewModelBase, IClosedCurveViewModel
{
    [ObservableProperty]
    public partial double X { get; set; } = 0;
    [ObservableProperty]
    public partial double Y { get; set; } = 0.1;
    [ObservableProperty]
    public partial double A { get; set; } = 0.6;
    [ObservableProperty]
    public partial double B { get; set; } = 0.4;
    [ObservableProperty]
    public partial double Alpha { get; set; } = 0;

    public IClosedCurve GetClosedCurve()
    {
        return new Ellipse(X, Y, A, B, double.DegreesToRadians(Alpha));
    }

    public string GetTomlString()
    {
        const string ret = """
                           normalized = true
                           shape = 'ellipse'
                           center = [0, 0.2]
                           a = 0.8
                           b = 0.6
                           alpha = 0
                           """;
        var model = Tomlyn.Toml.ToModel(ret);
        model["center"] = (double[]) [X, Y];
        model["a"] = A;
        model["b"] = B;
        model["alpha"] = Alpha;
        return Tomlyn.Toml.FromModel(model);
    }
}