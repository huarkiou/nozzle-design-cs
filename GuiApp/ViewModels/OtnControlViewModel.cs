using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using ScottPlot.Avalonia;
using Tomlyn;
using Tomlyn.Model;

namespace GuiApp.ViewModels;

public partial class OtnControlViewModel : ViewModelBase
{
    private bool _isRunning = false;
    private bool _irrotational = true;
    private bool _isAxisymmetric = true;
    private double _epsilon = 1e-6;
    private int _numCorrectionMax = 40;
    private int _numInletDivision = 101;
    private double _inletHeight = 1.0;
    private double _length = 4.0;
    private double _width = 1.0;
    private double _targetOutletHeight = double.NaN;
    private double _gamma = 1.4;
    private double _rg = 287.042;
    private double _totalPressure = 800000.0;
    private double _totalTemperature = 2000.0;
    private double _machNumber = 1.20;
    private double _inletTheta = 0.0;
    private double _radiusThroat = 0.0;
    private double _initialExpansionAngle = double.NaN;
    private double _pressureAmbient = 7000.0;

    public AvaPlot Displayer2D { get; } = new();

    // MOC Control
    public bool Irrotational
    {
        get => _irrotational;
        set
        {
            if (value == _irrotational) return;
            _irrotational = value;
            OnPropertyChanged();
        }
    }
    public bool IsAxisymmetric
    {
        get => _isAxisymmetric;
        set
        {
            if (value == _isAxisymmetric) return;
            _isAxisymmetric = value;
            OnPropertyChanged();
        }
    }
    public double Epsilon
    {
        get => _epsilon;
        set
        {
            if (value.Equals(_epsilon)) return;
            _epsilon = value;
            OnPropertyChanged();
        }
    }
    public int NumCorrectionMax
    {
        get => _numCorrectionMax;
        set
        {
            if (value == _numCorrectionMax) return;
            _numCorrectionMax = value;
            OnPropertyChanged();
        }
    }
    public int NumInletDivision
    {
        get => _numInletDivision;
        set
        {
            if (value == _numInletDivision) return;
            _numInletDivision = value;
            OnPropertyChanged();
        }
    }

    // Geometry
    public double InletHeight
    {
        get => _inletHeight;
        set
        {
            if (value.Equals(_inletHeight)) return;
            _inletHeight = value;
            OnPropertyChanged();
        }
    }
    public double Length
    {
        get => _length;
        set
        {
            if (value.Equals(_length)) return;
            _length = value;
            OnPropertyChanged();
        }
    }
    public double Width
    {
        get => _width;
        set
        {
            if (value.Equals(_width)) return;
            _width = value;
            OnPropertyChanged();
        }
    }
    public double TargetOutletHeight
    {
        get => _targetOutletHeight;
        set
        {
            if (value.Equals(_targetOutletHeight)) return;
            _targetOutletHeight = value;
            OnPropertyChanged();
        }
    }

    // Inlet
    public double Gamma
    {
        get => _gamma;
        set
        {
            if (value.Equals(_gamma)) return;
            _gamma = value;
            OnPropertyChanged();
        }
    }
    public double Rg
    {
        get => _rg;
        set
        {
            if (value.Equals(_rg)) return;
            _rg = value;
            OnPropertyChanged();
        }
    }
    public double TotalPressure
    {
        get => _totalPressure;
        set
        {
            if (value.Equals(_totalPressure)) return;
            _totalPressure = value;
            OnPropertyChanged();
        }
    }
    public double TotalTemperature
    {
        get => _totalTemperature;
        set
        {
            if (value.Equals(_totalTemperature)) return;
            _totalTemperature = value;
            OnPropertyChanged();
        }
    }
    public double MachNumber
    {
        get => _machNumber;
        set
        {
            if (value.Equals(_machNumber)) return;
            _machNumber = value;
            OnPropertyChanged();
        }
    }
    public double InletTheta
    {
        get => _inletTheta;
        set
        {
            if (value.Equals(_inletTheta)) return;
            _inletTheta = value;
            OnPropertyChanged();
        }
    }

    // Throat
    public double RadiusThroat
    {
        get => _radiusThroat;
        set
        {
            if (value.Equals(_radiusThroat)) return;
            _radiusThroat = value;
            OnPropertyChanged();
        }
    }
    public double InitialExpansionAngle
    {
        get => _initialExpansionAngle;
        set
        {
            if (value.Equals(_initialExpansionAngle)) return;
            _initialExpansionAngle = value;
            OnPropertyChanged();
        }
    }

    // Outlet
    public double PressureAmbient
    {
        get => _pressureAmbient;
        set
        {
            if (value.Equals(_pressureAmbient)) return;
            _pressureAmbient = value;
            OnPropertyChanged();
        }
    }

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
            ((TomlTable)otnConfigs["IO"])["output_prefix"] = "guiapp_";
            ((TomlTable)otnConfigs["IO"])["export_icemcfd"] = false;
        }
        await File.WriteAllTextAsync(tmpName, Toml.FromModel(otnConfigs));

        Console.WriteLine("{0}/{1}", tmpDir.FullName, tmpName);

        var process = new Process();
        process.StartInfo.WorkingDirectory = tmpDir.FullName;
#if DEBUG
        process.StartInfo.FileName = @"D:\Apps\study\nozzle_design\otn\OptimumNozzle.exe";
#else
        process.StartInfo.FileName = Path.Combine(AppContext.BaseDirectory, "tools", "OptimumNozzle.exe");
#endif
        process.StartInfo.Arguments = tmpName;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardInput = false;
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += (_, args) => Console.WriteLine(args.Data);
        process.Exited += async (s, _) =>
        {
            IsRunning = false;
            if ((s as Process)!.ExitCode != 0)
            {
                await MessageBoxManager.GetMessageBoxStandard("错误", "无法正常计算").ShowAsync();
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        await process.WaitForExitAsync();
        process.Close();

        var geoResultFile = Path.Combine(tmpDir.FullName, "guiapp_geo_all.dat");
        List<double> dataX = [];
        List<double> dataY = [];
        if (File.Exists(geoResultFile))
        {
            bool isStart = false;
            foreach (string line in File.ReadLines(geoResultFile))
            {
                if (line.Contains("ROW"))
                {
                    if (!isStart)
                    {
                        isStart = true;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    var r = line.Split(" ");
                    if (r.Length == 3)
                    {
                        double x = double.Parse(r[0]);
                        double y = double.Parse(r[1]);
                        dataX.Add(x);
                        dataY.Add(y);
                    }
                }
            }

            Displayer2D.Plot.Add.Scatter(dataX, dataY);
            Displayer2D.Plot.Add.Line(0, 0, 0, dataY[0]);
            Displayer2D.Plot.Add.Line(0, 0, dataX[^1], 0);
            Displayer2D.Plot.Add.Line(dataX[^1], 0, dataX[^1], dataY[^1]);
            Displayer2D.Plot.Axes.SquareUnits();
            Displayer2D.Plot.Axes.AutoScale();
            Displayer2D.Refresh();
        }
        else
        {
            await MessageBoxManager.GetMessageBoxStandard("错误", "无法正常计算").ShowAsync();
        }
    }
}