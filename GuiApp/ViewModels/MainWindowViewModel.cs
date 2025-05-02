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
                                     修改时间：2025-05-02
                                     """;

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
    public static async Task ShowCopyright()
    {
        await MessageBoxManager.GetMessageBoxStandard("说明", Copyright).ShowAsync();
    }
}