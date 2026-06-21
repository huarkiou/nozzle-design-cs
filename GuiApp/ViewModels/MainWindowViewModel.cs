using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using Serilog;

namespace GuiApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly OtnControlViewModel _otn;
    private readonly SltnControlViewModel _sltn;
    private readonly ILogger _logger;

    public MainWindowViewModel(
        OtnControlViewModel otnControlViewModel,
        SltnControlViewModel sltnControlViewModel)
    {
        _otn = otnControlViewModel;
        _sltn = sltnControlViewModel;
        _logger = Log.ForContext<MainWindowViewModel>();
    }

    private const int OtnIndex = 0;
    private const int SltnIndex = 1;

    private static string BuildTime
    {
        get
        {
            try
            {
                var path = Environment.ProcessPath;
                if (path is not null)
                {
                    var utcTime = File.GetLastWriteTimeUtc(path);
                    var beijingTz = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
                    var beijingTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, beijingTz);
                    return beijingTime.ToString("yyyy-MM-dd HH:mm") + " CST (UTC+8)";
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Warning(ex, "Failed to determine build time");
            }
            return "未知";
        }
    }

    private static string BuildConfiguration => BuildInfo.Configuration;

    private static string TargetFrameworkDisplay
    {
        get
        {
            var tf = BuildInfo.TargetFramework; // e.g., "net10.0"
            return tf.StartsWith("net") ? ".NET " + tf[3..] : tf;
        }
    }

    private static string GitHash => BuildInfo.GitHash;

    private static string PublishMode
    {
        get
        {
            var parts = new List<string>();
            if (string.Equals(BuildInfo.PublishAot, "true", StringComparison.OrdinalIgnoreCase))
                parts.Add("Native AOT");
            if (string.Equals(BuildInfo.SelfContained, "true", StringComparison.OrdinalIgnoreCase))
                parts.Add("Self-contained");
            if (string.Equals(BuildInfo.PublishTrimmed, "true", StringComparison.OrdinalIgnoreCase))
                parts.Add("Trimmed");
            return parts.Count > 0 ? string.Join(" | ", parts) : "Framework-dependent";
        }
    }

    private static string RuntimeVersion => RuntimeInformation.FrameworkDescription;

    private static string OSVersion => RuntimeInformation.OSDescription;

    private static string About
    {
        get
        {
            return "作者：Huarkiou\n" +
                   "GitHub：github.com/huarkiou\n" +
                   "构建配置：" + BuildConfiguration + "\n" +
                   "目标框架：" + TargetFrameworkDisplay + "\n" +
                   "Git 提交：" + GitHash + "\n" +
                   "发布模式：" + PublishMode + "\n" +
                   "运行时版本：" + RuntimeVersion + "\n" +
                   "操作系统：" + OSVersion + "\n" +
                   "编译时间：" + BuildTime;
        }
    }

    [ObservableProperty]
    public partial int CurrentIndex { get; set; }

    [RelayCommand]
    public async Task ExportResult()
    {
        switch (CurrentIndex)
        {
            case OtnIndex:
                await _otn.ExportResultAsync();
                break;
            case SltnIndex:
                await _sltn.ExportResultAsync();
                break;
        }
    }

    [RelayCommand]
    public async Task ShowCopyright()
    {
        var msBox = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
        {
            ContentTitle = "关于",
            ContentMessage = About,
            MinWidth = 480,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        });
        await msBox.ShowAsync();
    }
}
