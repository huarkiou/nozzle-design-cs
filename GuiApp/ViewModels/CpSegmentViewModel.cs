using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.Input;
using GuiApp.Models;

namespace GuiApp.ViewModels;

/// <summary>
/// 分段多项式比热容 ViewModel — 管理多个温度段的 Cp 系数
/// 激活时替代 Material.cp，生成 [[Material.cp_segments]] TOML 表
/// </summary>
public partial class CpSegmentViewModel : ViewModelBase
{
    public ObservableCollection<CpSegmentModel> Segments { get; } = [];

    /// <summary>
    /// 是否激活分段 Cp 模式（有区段时激活）
    /// </summary>
    public bool IsActive => Segments.Count > 0;

    /// <summary>
    /// 摘要文本："2 段"
    /// </summary>
    public string SummaryText
    {
        get
        {
            if (Segments.Count == 0)
                return "未启用";
            return $"分段多项式比热容 · {Segments.Count} 段";
        }
    }

    /// <summary>
    /// 添加一个新的温度区段：TMin = 上一段 TMax（首段默认 200），TMax = TMin + 1000
    /// </summary>
    [RelayCommand]
    private void AddSegment()
    {
        var last = Segments.OrderBy(s => s.TMin).LastOrDefault();
        var tMin = last?.TMax ?? 200.0;
        var segment = new CpSegmentModel { TMin = tMin, TMax = tMin + 1000 };
        segment.PropertyChanged += (_, _) => OnSegmentsChanged();
        Segments.Add(segment);
        SortAndNotify();
    }

    /// <summary>
    /// 移除指定区段
    /// </summary>
    [RelayCommand]
    private void RemoveSegment(CpSegmentModel? segment)
    {
        if (segment is not null && Segments.Remove(segment))
        {
            segment.PropertyChanged -= (_, _) => OnSegmentsChanged();
            SortAndNotify();
        }
    }

    /// <summary>
    /// 清空所有区段
    /// </summary>
    [RelayCommand]
    private void ClearAll()
    {
        foreach (var seg in Segments)
            seg.PropertyChanged -= (_, _) => OnSegmentsChanged();
        Segments.Clear();
        SortAndNotify();
    }

    /// <summary>
    /// 按 TMin 递增排序并通知 UI 刷新
    /// </summary>
    private void SortAndNotify()
    {
        var sorted = Segments.OrderBy(s => s.TMin).ToList();
        for (int i = 0; i < sorted.Count; i++)
        {
            var current = Segments.IndexOf(sorted[i]);
            if (current != i)
                Segments.Move(current, i);
        }
        OnSegmentsChanged();
    }

    private void OnSegmentsChanged()
    {
        OnPropertyChanged(nameof(IsActive));
        OnPropertyChanged(nameof(SummaryText));
    }

    /// <summary>
    /// 生成 Cp-T 图线数据：横轴为温度 (K)，纵轴为 Cp (J/(kg·K))
    /// </summary>
    public (double[] t, double[] cp) GeneratePlotData(int numPoints = 300)
    {
        if (Segments.Count == 0)
            return ([], []);

        var ordered = Segments.OrderBy(s => s.TMin).ToList();
        var tMin = ordered[0].TMin;
        var tMax = ordered[^1].TMax;
        var t = new double[numPoints];
        var cp = new double[numPoints];

        for (int i = 0; i < numPoints; i++)
        {
            t[i] = tMin + (tMax - tMin) * i / (numPoints - 1);
            // 找到 t 所在的区段并求值
            var segment = ordered.FirstOrDefault(s => t[i] >= s.TMin && t[i] <= s.TMax)
                          ?? (t[i] < tMin ? ordered[0] : ordered[^1]);
            cp[i] = segment.Evaluate(t[i]);
        }

        return (t, cp);
    }

    /// <summary>
    /// 生成 [[Material.cp_segments]] TOML 配置字符串
    /// </summary>
    public string BuildTomlString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("###### 分段多项式比热容 ######");
        foreach (var seg in Segments)
        {
            sb.AppendLine("[[Material.cp_segments]]");
            sb.AppendLine($"t_min = {FormatTomlDouble(seg.TMin)}");
            sb.AppendLine($"t_max = {FormatTomlDouble(seg.TMax)}");
            sb.AppendLine($"pos_coefficients = [{FormatTomlArray(seg.PosCoefficients)}]");
            sb.AppendLine($"neg_coefficients = [{FormatTomlArray(seg.NegCoefficients)}]");
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private static string FormatTomlDouble(double value)
    {
        if (double.IsNaN(value))
            return "nan";
        if (double.IsPositiveInfinity(value))
            return "+inf";
        if (double.IsNegativeInfinity(value))
            return "-inf";
        return value.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
    }

    private static string FormatTomlArray(double[] arr)
    {
        if (arr.Length == 0)
            return "";
        var parts = new string[arr.Length];
        for (int i = 0; i < arr.Length; i++)
        {
            parts[i] = FormatTomlDouble(arr[i]);
        }
        return string.Join(", ", parts);
    }
}
