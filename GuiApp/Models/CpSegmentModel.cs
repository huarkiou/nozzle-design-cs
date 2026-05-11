using CommunityToolkit.Mvvm.ComponentModel;

namespace GuiApp.Models;

/// <summary>
/// 分段多项式比热容模型 — 一个温度区段和对应的多项式系数
/// </summary>
public partial class CpSegmentModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Summary))]
    public partial double TMin { get; set; } = 300.0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Summary))]
    public partial double TMax { get; set; } = 1000.0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PosCoefficientsString))]
    public partial double[] PosCoefficients { get; set; } = [1004.5];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NegCoefficientsString))]
    public partial double[] NegCoefficients { get; set; } = [];

    /// <summary>
    /// 正幂次系数（逗号分隔字符串，用于 UI 绑定）
    /// index 0 = T⁰ 常数项，[1] = T¹，[2] = T²，...
    /// </summary>
    public string PosCoefficientsString
    {
        get => string.Join(", ", PosCoefficients);
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                PosCoefficients = [];
                return;
            }
            var parts = value.Split(',', System.StringSplitOptions.TrimEntries);
            var coeffs = new double[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                if (!double.TryParse(parts[i], out coeffs[i]))
                    coeffs[i] = 0.0;
            }
            PosCoefficients = coeffs;
        }
    }

    /// <summary>
    /// 负幂次系数（逗号分隔字符串，用于 UI 绑定）
    /// index 0 = T⁻¹，[1] = T⁻²，...
    /// </summary>
    public string NegCoefficientsString
    {
        get => string.Join(", ", NegCoefficients);
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                NegCoefficients = [];
                return;
            }
            var parts = value.Split(',', System.StringSplitOptions.TrimEntries);
            var coeffs = new double[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                if (!double.TryParse(parts[i], out coeffs[i]))
                    coeffs[i] = 0.0;
            }
            NegCoefficients = coeffs;
        }
    }

    /// <summary>
    /// 摘要："200–1000 K"
    /// </summary>
    public string Summary => $"{TMin}–{TMax} K";

    /// <summary>
    /// 多项式求值：Cp(T) = PosCoefficients[0] + PosCoefficients[1]·T + PosCoefficients[2]·T² + …
    ///                      + NegCoefficients[0]·T⁻¹ + NegCoefficients[1]·T⁻² + …
    /// </summary>
    public double Evaluate(double t)
    {
        var result = 0.0;
        var tPow = 1.0;
        foreach (var c in PosCoefficients)
        {
            result += c * tPow;
            tPow *= t;
        }
        var tInv = 1.0 / t;
        foreach (var c in NegCoefficients)
        {
            if (t == 0.0) return double.NaN;
            result += c * tInv;
            tInv /= t;
        }
        return result;
    }
}
