using CommunityToolkit.Mvvm.ComponentModel;

namespace GuiApp.ViewModels;

public partial class CrossSectionCircleViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial double X { get; set; } = 0;
    [ObservableProperty]
    public partial double Y { get; set; } = 0.1;
    [ObservableProperty]
    public partial double Radius { get; set; } = 0.4;

    public override string ToString()
    {
        const string ret = """
                           normalized = true
                           shape = 'circle'
                           center = [0, 0.2]
                           radius = 0.8
                           """;
        var model = Tomlyn.Toml.ToModel(ret);
        model["radius"] = Radius;
        model["center"] = (double[]) [X, Y];
        return Tomlyn.Toml.FromModel(model);
    }
}