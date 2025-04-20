using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using ScottPlot.Avalonia;
using Tomlyn;

namespace GuiApp.ViewModels;

public partial class SltnControlViewModel : ViewModelBase
{
    public AvaPlot Displayer2D { get; } = new();

    private DirectoryInfo? _currentDirectory;
    private const string ConfigFileName = "sltn_config.toml";
    private const string GeoResultFileName = "geo_all.dat";
    private const string OutputPrefix = "guiapp_";

    // View
    public bool IsRunning
    {
        get;
        private set
        {
            if (value == field) return;
            field = value;
            OnPropertyChanged();
        }
    }

    [RelayCommand]
    public void PreviewCrossSection()
    {
        List<double> dataX = [];
        List<double> dataY = [];
        Displayer2D.Plot.Add.Scatter(dataX, dataY);
        Displayer2D.Plot.Axes.SquareUnits();
        Displayer2D.Plot.Axes.AutoScale();
        Displayer2D.Refresh();
    }

    [RelayCommand]
    public async Task RunSltn()
    {
        IsRunning = true;

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