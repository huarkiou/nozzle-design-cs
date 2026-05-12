using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.DependencyInjection;
using GuiApp.ViewModels;
using GuiApp.Views;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using System.Diagnostics.CodeAnalysis;

namespace GuiApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        // configure logger
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
#else
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
#endif
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "logs/.log"), rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true)
            .CreateLogger();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var serviceCollection = new ServiceCollection();

        // Register ViewModels via DI (singleton — shared across tabs)
        serviceCollection.AddSingleton<OtnControlViewModel>();
        serviceCollection.AddSingleton<SltnControlViewModel>();
        serviceCollection.AddSingleton<MainWindowViewModel>();

        // StorageProvider requires TopLevel from MainWindow — defer resolution with Lazy
        var storageProviderHolder = new Lazy<IStorageProvider>(() =>
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                && desktop.MainWindow is not null)
            {
                var topLevel = TopLevel.GetTopLevel(desktop.MainWindow);
                return topLevel!.StorageProvider;
            }
            throw new InvalidOperationException("StorageProvider is not available before MainWindow is created.");
        });
        serviceCollection.AddSingleton(_ => storageProviderHolder.Value);

        // Build ONCE — Ioc.Default can only be configured once
        var serviceProvider = serviceCollection.BuildServiceProvider();
        Ioc.Default.ConfigureServices(serviceProvider);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }

    [UnconditionalSuppressMessage("Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = "<Pending>")]
    private static void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}