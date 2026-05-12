using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using MsBox.Avalonia;
using ScottPlot.Avalonia;
using Serilog;

namespace GuiApp.ViewModels;

public abstract class NozzleControlViewModelBase : ViewModelBase
{
    protected readonly ILogger _logger;
    protected DirectoryInfo? _currentDirectory;

    /// <summary>
    /// Subclass-specific TOML config file name (e.g. "otn_config.toml").
    /// </summary>
    protected abstract string ConfigFileName { get; }

    /// <summary>
    /// Shared 2D plot control for nozzle geometry / cross-section display.
    /// </summary>
    public AvaPlot Displayer2D { get; } = new();

    protected NozzleControlViewModelBase()
    {
        _logger = Log.ForContext(GetType());
    }

    /// <summary>
    /// Delete previous temp directory (if any) and create a fresh one.
    /// </summary>
    protected void PrepareTempDirectory(string prefix)
    {
        _currentDirectory?.Delete(true);
        _currentDirectory = Directory.CreateTempSubdirectory(prefix);
        Console.WriteLine("{0}", _currentDirectory.FullName);
    }

    /// <summary>
    /// Write a TOML string to <c>ConfigFileName</c> inside the working directory.
    /// </summary>
    protected async Task WriteConfigFileAsync(string content)
    {
        await File.WriteAllTextAsync(Path.Combine(_currentDirectory!.FullName, ConfigFileName), content);
    }

    /// <summary>
    /// Launch a backend .exe, capture stdout/stderr, and wait for exit.
    /// Returns the combined output, or <c>null</c> when the executable was not found.
    /// </summary>
    /// <param name="exeFileName">Relative exe name (e.g. "otn.exe").</param>
    /// <param name="logProcessName">Human-readable name for log messages.</param>
    /// <param name="onExited">Callback invoked when the process exits (typically re-enables <c>CanRun</c>).</param>
    protected async Task<string?> RunBackendProcessAsync(
        string exeFileName,
        string logProcessName,
        Action onExited)
    {
        string output = string.Empty;
        var process = new Process();
        process.StartInfo.WorkingDirectory = _currentDirectory!.FullName;
#if DEBUG
        process.StartInfo.FileName = $@"D:\Projects\Program\nozzle-design-rs\target\release\{exeFileName}";
#else
        process.StartInfo.FileName = Path.Combine(AppContext.BaseDirectory, "tools", exeFileName);
#endif
        if (!File.Exists(process.StartInfo.FileName))
        {
            _logger.Error("Backend executable not found: {Path}", process.StartInfo.FileName);
            await MessageBoxManager.GetMessageBoxStandard("错误", $"文件缺失：{process.StartInfo.FileName}").ShowAsync();
            return null;
        }

        process.StartInfo.Arguments = ConfigFileName;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
        process.StartInfo.RedirectStandardInput = false;
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += (_, args) => output += args.Data + "\r\n";
        process.ErrorDataReceived += (_, args) => output += args.Data + "\r\n";
        process.Exited += (_, _) => onExited();

        process.Start();
        _logger.Information("Started {ProcessName} (PID: {Pid}) in {WorkDir}",
            logProcessName, process.Id, _currentDirectory!.FullName);
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();
        _logger.Information("{ProcessName} exited with code {ExitCode}", logProcessName, process.ExitCode);
        process.Close();

        return output;
    }
}
