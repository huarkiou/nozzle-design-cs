using System.Collections.Generic;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GuiApp.Models;
using GuiApp.Views;

namespace GuiApp.ViewModels;

public partial class CrossSectionControlViewModel : ViewModelBase, IRecipient<NozzleSizeValueChangedMessages>
{
    public CrossSectionControlViewModel()
    {
        WeakReferenceMessenger.Default.Register<NozzleSizeValueChangedMessages, string>(this,
            nameof(OtnControlViewModel));
    }

    public void Receive(NozzleSizeValueChangedMessages valueChangedMessage)
    {
        (double hInlet, double hOutlet) = valueChangedMessage.Value;
        HNorm = Position switch
        {
            CrossSectionPosition.Inlet => hInlet,
            CrossSectionPosition.Outlet => hOutlet,
            _ => HNorm
        };
    }

    private double HNorm
    {
        get;
        set
        {
            if (CrossSectionInputer is not null)
                (CrossSectionInputer.DataContext as ClosedCurveViewModel)!.HNorm = value;
            field = value;
        }
    } = double.NaN;

    [ObservableProperty]
    public partial CrossSectionPosition Position { get; set; }

    public static List<string> CrossSectionShapes { get; } = ["圆", "椭圆", "矩形", "超椭圆", "自定义多边形", "自由", "NURBS(目前仍未实现)"];

    [ObservableProperty]
    public partial string SelectedCrossSectionType { get; set; } = CrossSectionShapes[0];

    partial void OnSelectedCrossSectionTypeChanged(string value)
    {
        if (value == CrossSectionShapes[0])
        {
            CrossSectionInputer = new CrossSectionCircle();
        }
        else if (value == CrossSectionShapes[1])
        {
            CrossSectionInputer = new CrossSectionEllipse();
        }
        else if (value == CrossSectionShapes[2])
        {
            CrossSectionInputer = new CrossSectionRectangular();
        }
        else if (value == CrossSectionShapes[3])
        {
            CrossSectionInputer = new CrossSectionSuperEllipse();
        }
        else if (value == CrossSectionShapes[4])
        {
            CrossSectionInputer = new CrossSectionPolygon();
        }
        else if (value == CrossSectionShapes[5])
        {
            CrossSectionInputer = null;
        }
        else if (value == CrossSectionShapes[6])
        {
            CrossSectionInputer = new CrossSectionNurbs();
        }
    }

    [ObservableProperty]
    public partial UserControl? CrossSectionInputer { get; set; } = new CrossSectionCircle();

    partial void OnCrossSectionInputerChanged(UserControl? value)
    {
        if (value is not null)
            (value.DataContext as ClosedCurveViewModel)!.HNorm = HNorm;
    }
}