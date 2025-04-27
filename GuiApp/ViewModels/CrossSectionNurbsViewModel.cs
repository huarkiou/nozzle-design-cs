using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Corelib.Geometry;

#pragma warning disable CS0414 // 字段已被赋值，但它的值从未被使用

namespace GuiApp.ViewModels;

public partial class CrossSectionNurbsViewModel : ClosedCurveViewModel
{
    [ObservableProperty]
    public partial double X { get; set; } = double.NaN;
    [ObservableProperty]
    public partial double Y { get; set; } = double.NaN;
    [ObservableProperty]
    public partial double Alpha { get; set; } = 0;
    [ObservableProperty]
    public partial string? NurbsFilePath { get; set; } = null;

    private double _degree = 3;
    private Point[]? _controlPoints = null;
    private double[]? _weight = null;
    private double[]? _knotVector = null; //m=n+p+1

    [RelayCommand]
    public async Task ChangeVerticesFile()
    {
        var storageProvider = Ioc.Default.GetService<IStorageProvider>()!;
        var file = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择多边形顶点数据文件",
            AllowMultiple = false,
            FileTypeFilter = [FilePickerFileTypes.All, FilePickerFileTypes.TextPlain, Models.FilePickerFileTypes.DatUG],
        });
        if (file.Count < 1)
        {
            return;
        }
        else
        {
            NurbsFilePath = file[0].Path.AbsolutePath;
        }

        if (!File.Exists(NurbsFilePath))
        {
            return;
        }

        var lines = File.ReadLines(NurbsFilePath);
        _controlPoints = lines.Select(s =>
        {
            var r = s.Split(',', ' ', '\t');
            return new Point(double.Parse(r[0]), double.Parse(r[1]));
        }).ToArray();
        if (_controlPoints.Length < 4)
        {
            _controlPoints = null;
        }
    }

    public override IClosedCurve? GetClosedCurve()
    {
        if (_controlPoints is null)
        {
            return null;
        }

        throw new NotImplementedException();
    }

    public override string GetTomlString()
    {
        const string ret = """
                           # 是否用基准流场的进/出口截面高度对坐标进行标准化到单位圆内(若为false，则后续长度单位均为m，默认为true)
                           normalized = true
                           # 形状
                           shape = 'nurbs'
                           # 存放描述该自定义形状坐标(最好为凸曲线否则不能保证成功)的文本文件
                           datasource = './nurbs.txt'
                           # 旋转角(长与θ=0轴的夹角)/°
                           alpha = 0
                           # 中心坐标(z,y) （*可选*：若不指定中心，则将自动计算所有点的形心作为中心）
                           center = [0, 0.2]
                           """;
        var model = Tomlyn.Toml.ToModel(ret);
        model["normalized"] = IsNormalized;
        if (double.IsFinite(X) && double.IsFinite(Y))
        {
            model["center"] = (double[]) [X, Y];
        }

        model["datasource"] = NurbsFilePath ?? string.Empty;
        model["alpha"] = Alpha;
        return Tomlyn.Toml.FromModel(model);
    }

    public override IClosedCurve? GetRawClosedCurve()
    {
        throw new NotImplementedException();
    }

    public override IClosedCurve? GetNormalizedClosedCurve()
    {
        throw new NotImplementedException();
    }

    protected override void OnNormalizedStateChanged()
    {
        if (IsNormalized)
        {
            X /= HNorm;
            Y /= HNorm;
        }
        else
        {
            X *= HNorm;
            Y *= HNorm;
        }

        throw new NotImplementedException();
    }
}