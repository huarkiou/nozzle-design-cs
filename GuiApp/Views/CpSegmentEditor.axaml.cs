using Avalonia.Controls;
using GuiApp.ViewModels;
using ScottPlot;
using ScottPlot.TickGenerators;
using System.Linq;

namespace GuiApp.Views;

public partial class CpSegmentEditor : Window
{
    public CpSegmentEditor()
    {
        InitializeComponent();
    }

    public CpSegmentEditor(CpSegmentViewModel vm) : this()
    {
        DataContext = vm;
        OkButton.Click += (_, _) => Close();

        // 初始化图线
        CpPlot.Plot.XLabel("温度 T (K)");
        CpPlot.Plot.YLabel("Cp (J/(kg·K))");
        var xAxis = CpPlot.Plot.Axes.Bottom;
        xAxis.TickGenerator = new NumericAutomatic();
        var yAxis = CpPlot.Plot.Axes.Left;
        yAxis.TickGenerator = new NumericAutomatic();
        CpPlot.Refresh();

        // 数据变化时更新图线
        vm.Segments.CollectionChanged += (_, _) => RefreshPlot();
        foreach (var seg in vm.Segments)
            seg.PropertyChanged += (_, _) => RefreshPlot();
        vm.PropertyChanged += (_, _) => RefreshPlot();

        RefreshPlot();
    }

    private void RefreshPlot()
    {
        var vm = (CpSegmentViewModel)DataContext!;
        if (vm.Segments.Count == 0)
        {
            CpPlot.Plot.Clear();
            CpPlot.Refresh();
            return;
        }

        var (t, cp) = vm.GeneratePlotData(300);
        CpPlot.Plot.Clear();
        var scatter = CpPlot.Plot.Add.ScatterLine(t, cp);
        scatter.LineWidth = 2;
        scatter.LineColor = ScottPlot.Color.FromColor(System.Drawing.Color.DarkBlue);

        // 显式设置坐标轴范围和整数刻度，避免字体渲染问题
        var dataTMin = t.Min();
        var dataTMax = t.Max();
        var xMargin = (dataTMax - dataTMin) * 0.05;
        CpPlot.Plot.Axes.SetLimitsX(dataTMin - xMargin, dataTMax + xMargin);
        CpPlot.Plot.Axes.AutoScaleY();

        var xAxis = CpPlot.Plot.Axes.Bottom;
        xAxis.TickGenerator = new NumericAutomatic();
        var yAxis = CpPlot.Plot.Axes.Left;
        yAxis.TickGenerator = new NumericAutomatic();

        CpPlot.Refresh();
    }
}
