using System.Collections.Generic;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using GuiApp.Views;

namespace GuiApp.ViewModels;

public partial class CrossSectionControlViewModel : ViewModelBase
{
    public static List<string> CrossSectionTypes { get; } = ["圆", "椭圆", "矩形", "超椭圆", "自定义多边形", "自由"];

    [ObservableProperty]
    public partial string SelectedCrossSectionType { get; set; } = CrossSectionTypes[0];

    partial void OnSelectedCrossSectionTypeChanged(string value)
    {
        if (value == CrossSectionTypes[0])
        {
            CrossSectionInputer = new CrossSectionCircle();
        }
        else if (value == CrossSectionTypes[1])
        {
            CrossSectionInputer = new CrossSectionEllipse();
        }
        else if (value == CrossSectionTypes[2])
        {
            CrossSectionInputer = new CrossSectionRectangular();
        }
    }

    [ObservableProperty]
    public partial UserControl? CrossSectionInputer { get; set; } = new CrossSectionCircle();
}