using System.Collections.Generic;
using Avalonia.Controls;
using GuiApp.Views;

namespace GuiApp.ViewModels;

public partial class CrossSectionControlViewModel : ViewModelBase
{
    public static List<string> CrossSectionTypes { get; } = ["圆", "椭圆", "矩形", "超椭圆", "自定义多边形", "自由"];
    public string SelectedCrossSectionType
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            if (value == CrossSectionTypes[0])
            {
                CrossSectionInputer = new CrossSectionCircle();
            }
            else if (value == CrossSectionTypes[1])
            {
                CrossSectionInputer = new CrossSectionEllipse();
            }

            OnPropertyChanged();
        }
    } = CrossSectionTypes[0];
    public UserControl? CrossSectionInputer
    {
        get;
        private set
        {
            if (value == field) return;
            field = value;
            OnPropertyChanged();
        }
    } = new CrossSectionCircle();
}