using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GuiApp.Models;
using GuiApp.Views;
using MsBox.Avalonia;
using ScottPlot.Avalonia;
using Tomlyn;
using Tomlyn.Model;

namespace GuiApp.ViewModels;

public partial class SltnControlViewModel : ViewModelBase, IRecipient<BaseFluidFieldMessage>
{
    public SltnControlViewModel()
    {
        WeakReferenceMessenger.Default.Register<BaseFluidFieldMessage, string>(this, nameof(OtnControlViewModel));
    }

    public CrossSectionControl Inlet { get; } = new() { Label = "进口截面形状：" };
    public CrossSectionControl Outlet { get; } = new() { Label = "出口截面形状：" };

    private DirectoryInfo? _currentDirectory;
    private const string ConfigFileName = "sltn_config.toml";
    private const string DatResultFileName = "model.dat";
    private const string ObjResultFileName = "model.obj";

    // Control
    [ObservableProperty]
    public partial int NumCircumferentialDivision { get; set; } = 66;

    [ObservableProperty]
    public partial int NumAxialDivision { get; set; } = 111;

    [ObservableProperty]
    public partial bool IsMonotonic { get; set; } = false;

    [ObservableProperty]
    public partial double WeightFunctionParameter { get; set; } = 0;

    // Base Fluid Field
    [ObservableProperty]
    public partial bool IsAxisymmetric { get; set; } = true;

    [ObservableProperty]
    public partial string? FieldDataSource { get; set; } = null;

    // View
    public AvaPlot Displayer2D { get; } = new();

    [ObservableProperty]
    public partial bool IsRunning { get; set; } = false;


    // Command
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

        if (FieldDataSource is null)
        {
            await MessageBoxManager.GetMessageBoxStandard("错误", "请先生成基准流场").ShowAsync();
            IsRunning = false;
            return;
        }

        var inletConfig = """
                          ###### 进口截面参数 ######
                          [Inlet]
                          # 定义截面形状

                          """;
        var type = (Inlet.DataContext as CrossSectionControlViewModel)!.CrossSectionInputer;
        if (type is CrossSectionCircle)
        {
            inletConfig += (type.DataContext as CrossSectionCircleViewModel)!.ToString();
        }

        var outletConfig = """
                           ###### 出口截面参数 ######
                           [Outlet]
                           # 定义截面形状

                           """;
        type = (Outlet.DataContext as CrossSectionControlViewModel)!.CrossSectionInputer;
        if (type is CrossSectionCircle)
        {
            outletConfig += (type.DataContext as CrossSectionCircleViewModel)!.ToString();
        }

        _currentDirectory?.Delete(true);
        _currentDirectory = Directory.CreateTempSubdirectory("guiapp-sltn-");
        Console.WriteLine("{0}", _currentDirectory.FullName);

        var sltnConfigs = Toml.ToModel("""
                                       ###### 控制参数 ######
                                       [Control]
                                       # 离散网格点数
                                       n_theta = 66
                                       n_axis = 111
                                       # 型面融合过渡时变化不单调的区域是否抹平
                                       monotonic = false
                                       ## 型面融合过渡权函数控制参数a
                                       # a = 0 为 f(x) = x，
                                       # a > 0 为 f(x) = arctan((2 * x - 1) * a) / arctan(a) / 2. + 0.5，
                                       # a < 0 为 f(x) = (tan(arctan(a) * (2 * x - 1)) / a + 1) / 2.
                                       weight_parameter_a = 0
                                       # 是否导入wavefront .obj文件用于预览
                                       export_obj = true

                                       ###### 基准流场参数 ######
                                       [BaseFluidField]
                                       # 基准流场类型：平面or轴对称
                                       axisymmetric = true
                                       # 顺流向追踪使用的轴对称基准流场数据文件
                                       datasource_inlet = './field_data.txt'
                                       # 逆流向追踪使用的轴对称基准流场数据文件
                                       datasource_outlet = './field_data.txt'

                                       """);
        {
            ((TomlTable)sltnConfigs["Control"])["n_theta"] = NumCircumferentialDivision;
            ((TomlTable)sltnConfigs["Control"])["n_axis"] = NumAxialDivision;
            ((TomlTable)sltnConfigs["Control"])["monotonic"] = IsMonotonic;
            ((TomlTable)sltnConfigs["Control"])["weight_parameter_a"] = WeightFunctionParameter;
            ((TomlTable)sltnConfigs["BaseFluidField"])["axisymmetric"] = IsAxisymmetric;
            ((TomlTable)sltnConfigs["BaseFluidField"])["datasource_inlet"] = FieldDataSource;
            ((TomlTable)sltnConfigs["BaseFluidField"])["datasource_outlet"] = FieldDataSource;
        }
        await File.WriteAllTextAsync(Path.Combine(_currentDirectory.FullName, ConfigFileName),
            Toml.FromModel(sltnConfigs) + inletConfig + outletConfig);

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

        if (!File.Exists(Path.Combine(_currentDirectory.FullName, DatResultFileName)))
        {
            await MessageBoxManager.GetMessageBoxStandard("错误", "无法正常计算，程序输出内容如下：\n" + output).ShowAsync();
        }
        else
        {
            await PreviewModel();
        }
    }

    [RelayCommand]
    public async Task PreviewModel()
    {
        IsRunning = true;
        if (_currentDirectory == null || !File.Exists(Path.Combine(_currentDirectory.FullName, ObjResultFileName)))
        {
            await MessageBoxManager.GetMessageBoxStandard("错误", "请先生成流线追踪喷管后再进行预览").ShowAsync();
            return;
        }

        var objResultFile = Path.Combine(_currentDirectory.FullName, ObjResultFileName);

        var objViewerProcess = new Process();
        objViewerProcess.StartInfo.WorkingDirectory = _currentDirectory.FullName;
#if DEBUG
        objViewerProcess.StartInfo.FileName = @"D:\Apps\study\nozzle_design\obj_viewer\WavefrontObjViewer.exe";
#else
            process.StartInfo.FileName = Path.Combine(AppContext.BaseDirectory, "tools", "WavefrontObjViewer.exe");
#endif
        objViewerProcess.StartInfo.Arguments = objResultFile;
        objViewerProcess.StartInfo.UseShellExecute = false;
        objViewerProcess.StartInfo.CreateNoWindow = false;
        objViewerProcess.StartInfo.RedirectStandardError = false;
        objViewerProcess.StartInfo.RedirectStandardOutput = true;
        objViewerProcess.EnableRaisingEvents = true;
        objViewerProcess.Exited += (_, _) => { IsRunning = false; };

        objViewerProcess.Start();
        await objViewerProcess.WaitForExitAsync();
        objViewerProcess.Close();
    }

    public void Receive(BaseFluidFieldMessage message)
    {
        FieldDataSource = message.Value;
    }
}