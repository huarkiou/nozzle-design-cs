using CommunityToolkit.Mvvm.ComponentModel;
using Corelib.Geometry;

namespace GuiApp.ViewModels;

public partial class CrossSectionRectangularViewModel : ViewModelBase, IClosedCurveViewModel
{
    [ObservableProperty]
    public partial double X { get; set; } = 0;
    [ObservableProperty]
    public partial double Y { get; set; } = 0.1;
    [ObservableProperty]
    public partial double Length { get; set; } = 0.6;
    [ObservableProperty]
    public partial double Width { get; set; } = 0.4;
    [ObservableProperty]
    public partial double Alpha { get; set; } = 0;

    public IClosedCurve GetClosedCurve()
    {
        return new Rectangular(X, Y, Length, Width, double.DegreesToRadians(Alpha));
    }

    public string GetTomlString()
    {
        const string ret = """
                           normalized = true
                           shape = 'rectangular'
                           center = [0, 0.2]
                           length = 0.8
                           width = 0.6
                           alpha = 0
                           """;
        var model = Tomlyn.Toml.ToModel(ret);
        model["center"] = (double[]) [X, Y];
        model["length"] = Length;
        model["width"] = Width;
        model["alpha"] = Alpha;
        return Tomlyn.Toml.FromModel(model);
    }
}