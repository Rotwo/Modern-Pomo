using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using modern_pomo.Interfaces;
using modern_pomo.Models;
using modern_pomo.Services;
using modern_pomo.ViewModels;
using modern_pomo.Views;

namespace modern_pomo;

public partial class App : Application
{
    public static ServiceProvider ServiceProvider { get; private set; }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collections = new ServiceCollection();

        collections.AddSingleton<MainViewModel>();
        collections.AddSingleton<SettingsViewModel>();
        
        collections.AddSingleton<IPomodoroTimerService, PomodoroTimerService>();
        collections.AddSingleton<IAppConfigService, AppConfigService>();

        var services = collections.BuildServiceProvider();
        ServiceProvider = services;
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = services.GetRequiredService<MainViewModel>(),
            };

            services.GetRequiredKeyedService<IAppConfigService>(null).LoadConfig();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
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