using Avalonia.Platform.Storage;

namespace GuiApp.Models;

public static class FilePickerFileTypes
{
    public static FilePickerFileType DatUG { get; } = new("UG NX .dat文件")
    {
        Patterns = ["*.dat"],
        MimeTypes = ["text/*"]
    };

    public static FilePickerFileType Obj { get; } = new("Wavefront .obj文件")
    {
        Patterns = ["*.obj"],
        MimeTypes = ["text/*"]
    };
}