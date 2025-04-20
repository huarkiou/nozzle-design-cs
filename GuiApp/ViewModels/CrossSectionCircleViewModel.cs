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
}