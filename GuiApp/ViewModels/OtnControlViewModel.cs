using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Corelib.Geometry;
using GuiApp.Models;
using MsBox.Avalonia;
using ScottPlot;
using ScottPlot.Avalonia;
using Tomlyn;
using Tomlyn.Model;
using FilePickerFileTypes = GuiApp.Models.FilePickerFileTypes;

namespace GuiApp.ViewModels;

public partial class OtnControlViewModel : ViewModelBase
{
    private DirectoryInfo? _currentDirectory;
    private const string ConfigFileName = "otn_config.toml";
    private const string GeoResultFileName = "geo_all.dat";
    private const string FieldResultFileName = "field_data.txt";
    private const string OutputPrefix = "guiapp_";

    // MOC Control
    [ObservableProperty]
    public partial bool Irrotational { get; set; } = true;
    public static string IsIrrotationalToolTip => "无旋特征线法/有旋特征线法\n注意：除测试用途以外均不应使用此选项，由于不知什么时候引入的BUG导致采用有旋特征线法大概率无法正常计算";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InletHeightToolTip))]
    [NotifyPropertyChangedFor(nameof(TargetOutletHeightToolTip))]
    public partial bool IsAxisymmetric { get; set; } = true;
    public static string IsAxisymmetricToolTip => "轴对称喷管/二元喷管";

    [ObservableProperty]
    public partial double Epsilon { get; set; } = 1e-6;
    public static string EpsilonToolTip => "允许误差\n注意：不应大于1e-4，一般建议取在范围1e-4至1e-8之间";

    [ObservableProperty]
    public partial int NumCorrectionMax { get; set; } = 40;
    public static string NumCorrectionMaxToolTip => "欧拉预估校正迭代过程的最大校正次数";

    [ObservableProperty]
    public partial int NumInletDivision { get; set; } = 101;
    public static string NumInletDivisionToolTip => "入口边界初始网格点数\n注意：若计算不收敛可尝试增大此参数";

    // Geometry
    private const string UnitMeterToolTip = "\n单位：米/m";
    [ObservableProperty]
    public partial double InletHeight { get; set; } = 1.0;
    public string InletHeightToolTip => (IsAxisymmetric ? "喷管进口半径" : "喷管进口高度") + UnitMeterToolTip;

    [ObservableProperty]
    public partial double Length { get; set; } = 4.0;
    public static string LengthToolTip => "目标喷管轴向长度" + UnitMeterToolTip;

    [ObservableProperty]
    public partial double Width { get; set; } = 1.0;
    public static string WidthToolTip => "喷管宽度、深度、z方向长度" + UnitMeterToolTip;

    [ObservableProperty]
    public partial double TargetOutletHeight { get; set; } = double.NaN;
    public string TargetOutletHeightToolTip => (IsAxisymmetric ? "喷管目标出口半径" : "喷管目标出口高度")
                                               + UnitMeterToolTip + "\n注意：若以最大推力为目标，对出口高度没有约束则应设置为NaN";

    // Inlet
    [ObservableProperty]
    public partial double Gamma { get; set; } = 1.4;
    public static string GammaToolTip => "气流平均比热比";

    [ObservableProperty]
    public partial double Rg { get; set; } = 287.042;
    public static string RgToopTip => "气流气体常数\n单位：J/(mol·K)";

    [ObservableProperty]
    public partial double TotalPressure { get; set; } = 800000.0;
    public static string TotalPressureToopTip => "进口气流平均总压\n单位：Pa";

    [ObservableProperty]
    public partial double TotalTemperature { get; set; } = 2000.0;
    public static string TotalTemperatureToopTip => "进口气流平均总温\n单位：K";

    [ObservableProperty]
    public partial double MachNumber { get; set; } = 1.20;
    public static string MachNumberToopTip => "进口气流平均马赫数\n注意：应大于等于1，其中恰好等于1时的处理方式与其他情况不一致";

    [ObservableProperty]
    public partial double InletTheta { get; set; } = 0.0;
    public static string InletThetaToopTip => "来流气流方向角\n单位：°\n注意：不应使用此参数";

    // Throat
    [ObservableProperty]
    public partial double RadiusThroat { get; set; } = 0.0;
    public static string RadiusThroatToopTip => "进口上壁面处过渡圆弧半径" + UnitMeterToolTip;

    [ObservableProperty]
    public partial double InitialExpansionAngle { get; set; } = double.NaN;
    public static string InitialExpansionAngleToopTip =>
        "初始膨胀角\n单位：°\n注意：若为负数或NaN则由程序自动迭代计算选取，这也会导致目标出口高度失效";

    // Outlet
    [ObservableProperty]
    public partial double PressureAmbient { get; set; } = 7000.0;
    public static string PressureAmbientToopTip => "设计出口背压\n单位：Pa";

    // View
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RunOtnCommand))]
    public partial bool CanRunOtn { get; set; } = true;
    public AvaPlot Displayer2D { get; } = new();


    [RelayCommand(CanExecute = nameof(CanRunOtn))]
    public async Task RunOtn()
    {
        CanRunOtn = false;

        _currentDirectory?.Delete(true);
        _currentDirectory = Directory.CreateTempSubdirectory("guiapp-otn-");
        Console.WriteLine("{0}", _currentDirectory.FullName);

        var otnConfigs = Toml.ToModel("""
                                      ###### 特征线法参数 ######
                                      [MOCControl]
                                      # 无旋特征线法(true)还是有旋特征线法(false)
                                      irrotational = true
                                      # false代表二维平面问题，true代表二维轴对称问题
                                      axisymmetric = true
                                      # 残差小于eps视为相等/收敛
                                      eps = 1e-6
                                      # 欧拉预估校正迭代的最大校正次数
                                      n_correction_max = 40
                                      # 入口边界划分网格点数 《===若计算发散可增大此参数重新尝试
                                      n_inlet = 101

                                      ###### 几何约束 ######
                                      [Geometry]
                                      # 喷管进口高度(二维平面)或者半径(轴对称)(m)
                                      height = 1
                                      # 喷管目标长度(m)
                                      length = 6
                                      # 喷管目标出口高度(m) *若以最大推力为目标，对出口高度没有约束则设置为nan*
                                      height_e = nan
                                      # 喷管的横向宽度(仅二维平面)(m)
                                      width = 1

                                      ###### 进口气流参数 ######
                                      [Inlet]
                                      # 气流比热比
                                      gamma = 1.4
                                      # 气流气体常数(J/(mol·K))
                                      Rg = 287.042
                                      # 来流总压(Pa)
                                      p_total = 800000.0
                                      # 来流总温(K)
                                      T_total = 2000.0
                                      # 来流马赫数
                                      Ma = 1.20
                                      # 来流气流方向角(°) 《===不建议使用此参数
                                      theta = 0

                                      ###### 喉部过渡段参数 ######
                                      [Throat]
                                      # 过渡圆弧半径(m)
                                      R_t = 0.0
                                      # 初始膨胀角(°) *若为负数或nan则由程序自动迭代计算选取，这也会导致[Geometry].height_e失效*
                                      theta = nan

                                      ###### 出口约束参数 ######
                                      [Outlet]
                                      # 设计出口背压(Pa)
                                      p_ambient = 7000

                                      ###### 输入输出 ######
                                      [IO]
                                      # 输出文件的前缀
                                      output_prefix = ""
                                      # 是否输出ICEM CFD formatted points
                                      export_icemcfd = false
                                      """);
        {
            ((TomlTable)otnConfigs["MOCControl"])["irrotational"] = Irrotational;
            ((TomlTable)otnConfigs["MOCControl"])["axisymmetric"] = IsAxisymmetric;
            ((TomlTable)otnConfigs["MOCControl"])["eps"] = Epsilon;
            ((TomlTable)otnConfigs["MOCControl"])["n_correction_max"] = NumCorrectionMax;
            ((TomlTable)otnConfigs["MOCControl"])["n_inlet"] = NumInletDivision;
            ((TomlTable)otnConfigs["Geometry"])["height"] = InletHeight;
            ((TomlTable)otnConfigs["Geometry"])["length"] = Length;
            ((TomlTable)otnConfigs["Geometry"])["height_e"] = TargetOutletHeight;
            ((TomlTable)otnConfigs["Geometry"])["width"] = Width;
            ((TomlTable)otnConfigs["Inlet"])["gamma"] = Gamma;
            ((TomlTable)otnConfigs["Inlet"])["Rg"] = Rg;
            ((TomlTable)otnConfigs["Inlet"])["p_total"] = TotalPressure;
            ((TomlTable)otnConfigs["Inlet"])["T_total"] = TotalTemperature;
            ((TomlTable)otnConfigs["Inlet"])["Ma"] = MachNumber;
            ((TomlTable)otnConfigs["Inlet"])["theta"] = InletTheta;
            ((TomlTable)otnConfigs["Throat"])["R_t"] = RadiusThroat;
            ((TomlTable)otnConfigs["Throat"])["theta"] = InitialExpansionAngle;
            ((TomlTable)otnConfigs["Outlet"])["p_ambient"] = PressureAmbient;
            ((TomlTable)otnConfigs["Outlet"])["p_ambient"] = PressureAmbient;
            ((TomlTable)otnConfigs["IO"])["output_prefix"] = OutputPrefix;
            ((TomlTable)otnConfigs["IO"])["export_icemcfd"] = false;
        }
        await File.WriteAllTextAsync(Path.Combine(_currentDirectory.FullName, ConfigFileName),
            Toml.FromModel(otnConfigs));

        string output = string.Empty;
        var process = new Process();
        process.StartInfo.WorkingDirectory = _currentDirectory.FullName;
#if DEBUG
        process.StartInfo.FileName = @"D:\Apps\study\nozzle_design\otn\OptimumNozzle.exe";
#else
        process.StartInfo.FileName = Path.Combine(AppContext.BaseDirectory, "tools", "OptimumNozzle.exe");
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
        process.Exited += (_, _) => { CanRunOtn = true; };

        process.Start();
        process.BeginOutputReadLine();
        await process.WaitForExitAsync();
        process.Close();

        var fieldResultFile = Path.Combine(_currentDirectory!.FullName, OutputPrefix + FieldResultFileName);
        if (File.Exists(fieldResultFile))
        {
            WeakReferenceMessenger.Default.Send(new BaseFieldValueChangedMessages(Path.Combine(_currentDirectory.FullName,
                OutputPrefix + FieldResultFileName)), nameof(OtnControlViewModel));
        }

        await PreviewOtn(output);
    }

    private async Task PreviewOtn(string output)
    {
        Displayer2D.Plot.Clear();
        Displayer2D.Plot.XLabel("x/m");
        Displayer2D.Plot.YLabel("y/m");

        var geoResultFile = Path.Combine(_currentDirectory!.FullName, OutputPrefix + GeoResultFileName);
        if (!File.Exists(geoResultFile))
        {
            await MessageBoxManager.GetMessageBoxStandard("错误", "无法正常计算，程序输出内容如下：\n" + output).ShowAsync();
            return;
        }

        var data = PointExtensions.LoadDat(geoResultFile).First();
        var dataX = data.Select(p => p.X).ToArray();
        var dataY = data.Select(p => p.Y).ToArray();

        var wallU = Displayer2D.Plot.Add.ScatterLine(dataX, dataY);
        wallU.LineColor = Color.FromColor(System.Drawing.Color.Black);
        wallU.LinePattern = LinePattern.Solid;
        wallU.LineWidth = 2f;
        var wallL = Displayer2D.Plot.Add.Line(0, 0, 0, dataY[0]);
        wallL.LineColor = Color.FromColor(System.Drawing.Color.Black);
        wallL.LinePattern = LinePattern.DenselyDashed;
        wallL.LineWidth = 1.5f;
        var wallD = Displayer2D.Plot.Add.Line(0, 0, dataX[^1], 0);
        wallD.LineColor = Color.FromColor(System.Drawing.Color.Black);
        wallD.LinePattern = LinePattern.DenselyDashed;
        wallD.LineWidth = 1.5f;
        var wallR = Displayer2D.Plot.Add.Line(dataX[^1], 0, dataX[^1], dataY[^1]);
        wallR.LineColor = Color.FromColor(System.Drawing.Color.Black);
        wallR.LinePattern = LinePattern.DenselyDashed;
        wallR.LineWidth = 1.5f;

        Displayer2D.Plot.Axes.SquareUnits();
        Displayer2D.Plot.Axes.AutoScale();
        Displayer2D.Refresh();

        WeakReferenceMessenger.Default.Send(new NozzleSizeValueChangedMessages(dataY[0] - 0, dataY[^1] - 0),
            nameof(OtnControlViewModel));
    }

    public async Task ExportResultAsync()
    {
        var geoResultFile = _currentDirectory is null
            ? null
            : Path.Combine(_currentDirectory.FullName, OutputPrefix + GeoResultFileName);
        if (File.Exists(geoResultFile))
        {
            var storageProvider = Ioc.Default.GetService<IStorageProvider>()!;

            // 启动异步操作以打开对话框。
            var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "导出（流线追踪喷管）",
                DefaultExtension = ".dat",
                SuggestedFileName = "otn.dat",
                ShowOverwritePrompt = true,
                FileTypeChoices = [FilePickerFileTypes.DatUG]
            });

            if (file is not null)
            {
                File.Copy(geoResultFile, file.Path.AbsolutePath, true);
                await MessageBoxManager.GetMessageBoxStandard("提示", "导出成功").ShowAsync();
            }
        }
        else
        {
            await MessageBoxManager.GetMessageBoxStandard("错误", "请先计算后再导出").ShowAsync();
        }
    }
}