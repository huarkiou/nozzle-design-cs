namespace GuiApp.ViewModels;

public class CrossSectionCircleViewModel : ViewModelBase
{
    public double X
    {
        get;
        set
        {
            if (value.Equals(field)) return;
            field = value;
            OnPropertyChanged();
        }
    } = 0;
    public double Y
    {
        get;
        set
        {
            if (value.Equals(field)) return;
            field = value;
            OnPropertyChanged();
        }
    } = 0;
    public double Radius
    {
        get;
        set
        {
            if (value.Equals(field)) return;
            field = value;
            OnPropertyChanged();
        }
    } = 1;

    public override string ToString()
    {
        const string ret = """
                           # 定义截面形状
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