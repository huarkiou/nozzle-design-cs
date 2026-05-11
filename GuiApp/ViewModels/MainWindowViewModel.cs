using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;

namespace GuiApp.ViewModels;

public partial class MainWindowViewModel(
    OtnControlViewModel otnControlViewModel,
    SltnControlViewModel sltnControlViewModel)
    : ViewModelBase
{
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
                    return File.GetLastWriteTime(path).ToString("yyyy-MM-dd HH:mm");
            }
            catch { }
            return "未知";
        }
    }

    private static string Copyright =>
        $"作者：Huarkiou\n" +
        $"GitHub：github.com/huarkiou\n" +
        $"编译时间：{BuildTime}";

    [ObservableProperty]
    public partial int CurrentIndex { get; set; }

    [RelayCommand]
    public async Task ExportResult()
    {
        switch (CurrentIndex)
        {
            case OtnIndex:
                await otnControlViewModel.ExportResultAsync();
                break;
            case SltnIndex:
                await sltnControlViewModel.ExportResultAsync();
                break;
        }
    }

    [RelayCommand]
    public async Task ShowCopyright()
    {
        await MessageBoxManager.GetMessageBoxStandard("关于", Copyright).ShowAsync();
    }
}
