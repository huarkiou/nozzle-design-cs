using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Corelib.Geometry;

namespace GuiApp.ViewModels;

public partial class CrossSectionPolygonViewModel : ClosedCurveViewModel
{
    [ObservableProperty]
    public partial double X { get; set; } = double.NaN;
    public static string XToolTip { get; set; } = "中心x坐标";

    [ObservableProperty]
    public partial double Y { get; set; } = double.NaN;
    public static string YToolTip { get; set; } = "中心y坐标";
    [ObservableProperty]
    public partial string? VerticesFilePath { get; set; } = null;
    public static string VerticesFilePathToolTip { get; set; } =
        "顶点数据文件路径\n注意：文件英文纯文本文件，顶点的二维坐标用英文逗号,、空格 、或者制表符\t分隔，每行一个\n示例：\n\t-1, -1\n\t 1, -1\n\t 1,  1\n\t-1,  1";
    [ObservableProperty]
    public partial double Alpha { get; set; } = 0;
    public static string AlphaToolTip { get; set; } = "旋转角α";
    [ObservableProperty]
    public partial ObservableCollection<Point> Vertices { get; set; } = [];

    private Point[]? _vertices = null;

    partial void OnVerticesFilePathChanged(string? value)
    {
        Vertices.Clear();
        if (value is null)
        {
            return;
        }

        try
        {
            _vertices = PointExtensions.LoadTxt(value);
        }
        catch (System.Exception)
        {
            _vertices = null;
            return;
        }

        if (_vertices.Length < 3)
        {
            _vertices = null;
            return;
        }

        foreach (var p in _vertices)
        {
            Vertices.Add(p);
        }
    }

    [RelayCommand]
    public async Task ChangeVerticesFile()
    {
        var file = await Ioc.Default.GetService<IStorageProvider>()!.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "选择多边形顶点数据文件",
                AllowMultiple = false,
                FileTypeFilter =
                    [FilePickerFileTypes.All, FilePickerFileTypes.TextPlain, Models.FilePickerFileTypes.DatUG],
            });
        if (file.Count < 1 || VerticesFilePath == file[0].Path.AbsolutePath)
        {
            return;
        }

        VerticesFilePath = file[0].Path.AbsolutePath;
    }

    public override string GetTomlString()
    {
        const string ret = """
                           # 是否用基准流场的进/出口截面高度对坐标进行标准化到单位圆内(若为false，则后续长度单位均为m，默认为true)
                           normalized = true
                           # 形状
                           shape = 'userdefined'
                           # 存放描述该自定义形状坐标(最好为凸曲线否则不能保证成功)的文本文件，每行一个二维坐标值以空格分隔，第一列为z坐标，第二列为y坐标
                           datasource = './shapepoints.txt'
                           # 旋转角(长与θ=0轴的夹角)/°
                           alpha = 0
                           # 中心坐标(z,y) （*可选*：若不指定中心，则将自动计算所有点的形心作为中心）
                           center = [0, 0.25]
                           """;
        var model = Tomlyn.Toml.ToModel(ret);
        model["normalized"] = IsNormalized;
        if (double.IsFinite(X) && double.IsFinite(Y))
        {
            model["center"] = (double[]) [X, Y];
        }

        model["datasource"] = VerticesFilePath ?? string.Empty;
        model["alpha"] = Alpha;
        return Tomlyn.Toml.FromModel(model);
    }

    public override IClosedCurve? GetClosedCurve()
    {
        if (_vertices is null || _vertices.Length < 3)
        {
            return null;
        }

        return new Polygon(X, Y, _vertices, double.DegreesToRadians(Alpha));
    }

    public override IClosedCurve? GetRawClosedCurve()
    {
        if (_vertices is null || _vertices.Length < 3)
        {
            return null;
        }

        return IsNormalized
            ? new Polygon(X * HNorm, Y * HNorm, _vertices, double.DegreesToRadians(Alpha))
            : GetClosedCurve();
    }

    public override IClosedCurve? GetNormalizedClosedCurve()
    {
        if (_vertices is null || _vertices.Length < 3)
        {
            return null;
        }

        return IsNormalized
            ? GetClosedCurve()
            : new Polygon(X / HNorm, Y / HNorm, _vertices, double.DegreesToRadians(Alpha));
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
    }
}