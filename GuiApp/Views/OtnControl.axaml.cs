using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace GuiApp.Views;

public partial class OtnControl : UserControl
{
    public OtnControl()
    {
        InitializeComponent();
        double[] dataX = [1, 2, 3, 4, 5];
        double[] dataY = [1, 1.5, 3, 4.5, 5];

        Displayer2D.Plot.Add.Scatter(dataX, dataY);
        Displayer2D.Plot.Axes.SquareUnits();
        Displayer2D.Plot.Axes.AutoScale();
        Displayer2D.Refresh();
    }
}