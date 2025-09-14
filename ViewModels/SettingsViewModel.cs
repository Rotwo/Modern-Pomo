using System;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using modern_pomo.Interfaces;
using modern_pomo.Models;

namespace modern_pomo.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly IAppConfigService _appConfigService;
    private readonly IPomodoroTimerService _pomodoroTimerService;

    public SettingsViewModel(IAppConfigService appConfigService, IPomodoroTimerService pomodoroTimerService)
    {
        _appConfigService = appConfigService;
        _pomodoroTimerService = pomodoroTimerService;

    }
    
    [ObservableProperty]
    private decimal _focusModeDuration = 25;

    [ObservableProperty] 
    private decimal _breakModeDuration = 5;

    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(PomodoroCycles))]
    private int _pomodoroCyclesDecimal = 1;
    
    public int? PomodoroCycles => (int?)PomodoroCyclesDecimal;

    [RelayCommand]
    private void SaveConfig(Window window)
    {
        Console.WriteLine("Pressed button!");
        
        var focusModeDurationTimeSpan = TimeSpan.FromMinutes((double)FocusModeDuration);
        var breakModeDurationTimeSpan = TimeSpan.FromMinutes((double)BreakModeDuration);
        var pomodoroCycles = PomodoroCycles;
        
        _appConfigService.Config.FocusDuration = focusModeDurationTimeSpan;
        _appConfigService.Config.BreakDuration = breakModeDurationTimeSpan;
        _appConfigService.Config.MaxCycles = pomodoroCycles ?? 1;
        
        _appConfigService.SaveConfig();
        
        _pomodoroTimerService.SetFocusTime(_appConfigService.Config.FocusDuration);
        _pomodoroTimerService.SetBreakTime(_appConfigService.Config.BreakDuration);
        _pomodoroTimerService.SetCycles(_appConfigService.Config.MaxCycles);
        
        window.Close();
    }
}