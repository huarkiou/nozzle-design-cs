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
    private const string Copyright = """
                                     作者：Huarkiou
                                     个人主页：https://github.com/huarkiou
                                     修改时间：2025-04-30
                                     """;

    [ObservableProperty]
    public partial int CurrentIndex { get; set; }

    [RelayCommand]
    public async Task ExportResult()
    {
        if (CurrentIndex == OtnIndex)
        {
            await otnControlViewModel.ExportResultAsync();
        }
        else if (CurrentIndex == SltnIndex)
        {
            await sltnControlViewModel.ExportResultAsync();
        }
    }

    [RelayCommand]
    public async Task ShowCopyright()
    {
        await MessageBoxManager.GetMessageBoxStandard("说明", Copyright).ShowAsync();
    }
}