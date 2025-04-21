using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GuiApp.Views;
using MsBox.Avalonia;
using ScottPlot.Avalonia;
using Tomlyn;

namespace GuiApp.ViewModels;

public partial class SltnControlViewModel : ViewModelBase
{
    public CrossSectionControl Inlet { get; } = new() { Label = "进口截面形状：" };
    public CrossSectionControl Outlet { get; } = new() { Label = "出口截面形状：" };

    private DirectoryInfo? _currentDirectory;
    private const string ConfigFileName = "sltn_config.toml";
    private const string GeoResultFileName = "geo_all.dat";
    private const string OutputPrefix = "guiapp_";

    // View
    public AvaPlot Displayer2D { get; } = new();
    [ObservableProperty]
    public partial bool IsRunning { get; set; } = false;

    [RelayCommand]
    public void PreviewCrossSection()
    {
        Displayer2D.Plot.Clear();
        Displayer2D.Plot.Add.Circle(0, 0, 1);

        var type = (Inlet.DataContext as CrossSectionControlViewModel)!.CrossSectionInputer;
        if (type is CrossSectionCircle)
        {
            var circle = (type.DataContext as CrossSectionCircleViewModel)!;
            Displayer2D.Plot.Add.Circle(circle.X, circle.Y, circle.Radius);
        }

        type = (Outlet.DataContext as CrossSectionControlViewModel)!.CrossSectionInputer;
        if (type is CrossSectionCircle)
        {
            var circle = (type.DataContext as CrossSectionCircleViewModel)!;
            Displayer2D.Plot.Add.Circle(circle.X, circle.Y, circle.Radius);
        }

        Displayer2D.Plot.Axes.SquareUnits();
        Displayer2D.Plot.Axes.AutoScale();
        Displayer2D.Refresh();
    }

    [RelayCommand]
    public async Task RunSltn()
    {
        IsRunning = true;

        var config = """
                     ###### 进口截面参数 ######
                     [Inlet]
                     """;
        var type = (Inlet.DataContext as CrossSectionControlViewModel)!.CrossSectionInputer;
        if (type is CrossSectionCircle)
        {
            config += (type.DataContext as CrossSectionCircleViewModel)!.ToString();
        }

        Console.WriteLine(config);

        _currentDirectory?.Delete(true);
        _currentDirectory = Directory.CreateTempSubdirectory("guiapp-sltn-");
        Console.WriteLine("{0}", _currentDirectory.FullName);

        var sltnConfigs = Toml.ToModel("""

                                       """);
        {
            // ((TomlTable)otnConfigs["MOCControl"])["irrotational"] = Irrotational;
        }
        await File.WriteAllTextAsync(Path.Combine(_currentDirectory.FullName, ConfigFileName),
            Toml.FromModel(sltnConfigs));

        string output = string.Empty;
        var process = new Process();
        process.StartInfo.WorkingDirectory = _currentDirectory.FullName;
#if DEBUG
        process.StartInfo.FileName = @"D:\Apps\study\nozzle_design\sltn\StreamlineTraceNozzle.exe";
#else
        process.StartInfo.FileName = Path.Combine(AppContext.BaseDirectory, "tools", "StreamlineTraceNozzle.exe");
#endif
        process.StartInfo.Arguments = ConfigFileName;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
        process.StartInfo.RedirectStandardInput = false;
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += (_, args) => output += args.Data + "\r\n";
        process.Exited += (_, _) => { IsRunning = false; };

        process.Start();
        process.BeginOutputReadLine();
        await process.WaitForExitAsync();
        process.Close();

        if (!File.Exists(OutputPrefix + GeoResultFileName))
        {
            await MessageBoxManager.GetMessageBoxStandard("错误", "无法正常计算，程序输出内容如下：\n" + output).ShowAsync();
            Displayer2D.Plot.Clear();
        }
    }
}