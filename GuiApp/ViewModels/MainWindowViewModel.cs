using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GuiApp.ViewModels;

public partial class MainWindowViewModel(
    OtnControlViewModel otnControlViewModel,
    SltnControlViewModel sltnControlViewModel)
    : ViewModelBase
{
    private const int OtnIndex = 0;
    private const int SltnIndex = 1;

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
}