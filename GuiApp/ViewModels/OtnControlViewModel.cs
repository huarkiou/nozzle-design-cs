using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace GuiApp.ViewModels;

public partial class OtnControlViewModel : ViewModelBase
{
    private bool _isRunning = false;
    // MOC Control
    public bool Irrotational { get; set; } = true;
    public bool IsAxisymmetric { get; set; } = true;
    public double Epsilon { get; set; } = 1e-6;
    public int NumCorrectionMax { get; set; } = 40;
    public int NumInletDivision { get; set; } = 101;

    // Geometry
    public double InletHeight { get; set; } = 1.0;
    public double Length { get; set; } = 4.0;
    public double Width { get; set; } = 1.0;
    public double TargetOutletHeight { get; set; } = double.NaN;

    // Inlet
    public double Gamma { get; set; } = 1.4;
    public double Rg { get; set; } = 287.042;
    public double TotalPressure { get; set; } = 800000.0;
    public double TotalTemperature { get; set; } = 2000.0;
    public double MachNumber { get; set; } = 1.20;
    public double InletTheta { get; set; } = 0.0;

    // Throat
    public double RadiusThroat { get; set; } = 0.0;
    public double InitialExpansionAngle { get; set; } = double.NaN;

    // Outlet
    public double PressureAmbient { get; set; } = 7000.0;

    // View
    public bool IsRunning
    {
        get => _isRunning;
        private set
        {
            if (value == _isRunning) return;
            _isRunning = value;
            OnPropertyChanged();
        }
    }

    [RelayCommand]
    public async Task RunOtn()
    {
        IsRunning = true;

        var tmpDir = Directory.CreateTempSubdirectory("otn");
        Directory.SetCurrentDirectory(tmpDir.FullName);

        var tmpName = Guid.NewGuid().ToString("N") + ".toml";
        await File.WriteAllTextAsync(tmpName, """
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

        Console.WriteLine("{0}/{1}", tmpDir.FullName, tmpName);

        var process = new Process();
        process.StartInfo.WorkingDirectory = tmpDir.FullName;
        process.StartInfo.FileName = @"D:\Apps\study\nozzle_design\otn\OptimumNozzle.exe";
        process.StartInfo.Arguments = tmpName;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardInput = false;
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += (_, args) => Console.WriteLine(args.Data);
        process.Exited += (_, _) => { IsRunning = false; };
        process.Start();
        process.BeginOutputReadLine();
        await process.WaitForExitAsync();
        process.Close();
    }
}