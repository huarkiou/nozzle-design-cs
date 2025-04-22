using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Corelib.Geometry;
using GuiApp.Models;
using GuiApp.Views;
using MsBox.Avalonia;
using ScottPlot.Avalonia;
using Tomlyn;
using Tomlyn.Model;
using FilePickerFileTypes = GuiApp.Models.FilePickerFileTypes;

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
    [NotifyCanExecuteChangedFor(nameof(RunSltnCommand), nameof(PreviewModelCommand))]
    public partial bool CanRunSltn { get; set; } = true;

    private (double[], double[]) GetXYFromInputer(UserControl? type)
    {
        IClosedCurve? c = null;
        switch (type)
        {
            case null:
                break;
            case CrossSectionCircle:
            {
                var circle = (type.DataContext as CrossSectionCircleViewModel)!;
                c = new Circle(circle.X, circle.Y, circle.Radius);
                break;
            }
        }

        if (c is not null)
        {
            var points = c.GeneratePoints(NumCircumferentialDivision);
            var pointsCircle = new Point[points.Length + 1];
            points.CopyTo(pointsCircle, 0);
            pointsCircle[points.Length] = points[0];
            var dataX = pointsCircle.Select(p => p.X).ToArray();
            var dataY = pointsCircle.Select(p => p.Y).ToArray();
            return (dataX, dataY);
        }

        return ([], []);
    }

    // Command
    [RelayCommand]
    public void PreviewCrossSection()
    {
        Displayer2D.Plot.Clear();
        Displayer2D.Plot.Add.Circle(0, 0, 1);

        (double[] dataX, double[] dataY) =
            GetXYFromInputer((Inlet.DataContext as CrossSectionControlViewModel)!.CrossSectionInputer);
        Displayer2D.Plot.Add.ScatterLine(dataX, dataY);

        (dataX, dataY) =
            GetXYFromInputer((Outlet.DataContext as CrossSectionControlViewModel)!.CrossSectionInputer);
        Displayer2D.Plot.Add.ScatterLine(dataX, dataY);

        Displayer2D.Plot.Axes.SquareUnits();
        Displayer2D.Plot.Axes.AutoScale();
        Displayer2D.Refresh();
    }

    [RelayCommand]
    public async Task RunSltn()
    {
        if (FieldDataSource is null)
        {
            await MessageBoxManager.GetMessageBoxStandard("错误", "请先生成基准流场").ShowAsync();
            return;
        }

        CanRunSltn = false;

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
        if (!File.Exists(process.StartInfo.FileName))
        {
            await MessageBoxManager.GetMessageBoxStandard("错误", $"文件缺失：{process.StartInfo.FileName}").ShowAsync();
        }

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
        process.Exited += (_, _) => { CanRunSltn = true; };

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
        if (_currentDirectory == null || !File.Exists(Path.Combine(_currentDirectory.FullName, ObjResultFileName)))
        {
            await MessageBoxManager.GetMessageBoxStandard("错误", "请先生成流线追踪喷管后再进行预览").ShowAsync();
            return;
        }

        CanRunSltn = false;

        var objResultFile = Path.Combine(_currentDirectory.FullName, ObjResultFileName);

        var process = new Process();
        process.StartInfo.WorkingDirectory = _currentDirectory.FullName;
#if DEBUG
        process.StartInfo.FileName = @"D:\Apps\study\nozzle_design\obj_viewer\WavefrontObjViewer.exe";
#else
        process.StartInfo.FileName = Path.Combine(AppContext.BaseDirectory, "tools", "WavefrontObjViewer.exe");
#endif
        if (!File.Exists(process.StartInfo.FileName))
        {
            await MessageBoxManager.GetMessageBoxStandard("错误", $"文件缺失：{process.StartInfo.FileName}").ShowAsync();
        }

        process.StartInfo.Arguments = objResultFile;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = false;
        process.StartInfo.RedirectStandardError = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.EnableRaisingEvents = true;
        process.Exited += (_, _) => { CanRunSltn = true; };

        process.Start();
        await process.WaitForExitAsync();
        process.Close();
    }

    public void Receive(BaseFluidFieldMessage message)
    {
        FieldDataSource = message.Value;
    }

    public async Task ExportResultAsync()
    {
        var datResultFile = _currentDirectory is not null
            ? Path.Combine(_currentDirectory.FullName, DatResultFileName)
            : null;
        var objResultFile = _currentDirectory is not null
            ? Path.Combine(_currentDirectory.FullName, ObjResultFileName)
            : null;

        if (File.Exists(datResultFile) && File.Exists(objResultFile))
        {
            var storageProvider = Ioc.Default.GetService<IStorageProvider>()!;
            var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "导出（流线追踪喷管）",
                DefaultExtension = ".dat",
                SuggestedFileName = "sltn.dat",
                ShowOverwritePrompt = true,
                FileTypeChoices = [FilePickerFileTypes.DatUG, FilePickerFileTypes.Obj]
            });

            if (file is not null)
            {
                if (file.Path.AbsolutePath.EndsWith(".obj"))
                {
                    File.Copy(objResultFile, file.Path.AbsolutePath, true);
                }
                else if (file.Path.AbsolutePath.EndsWith(".dat"))
                {
                    File.Copy(datResultFile, file.Path.AbsolutePath, true);
                }

                await MessageBoxManager.GetMessageBoxStandard("提示", "导出成功").ShowAsync();
            }
        }
        else
        {
            await MessageBoxManager.GetMessageBoxStandard("错误", "请先计算后再导出").ShowAsync();
        }
    }
}