using System;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using modern_pomo.Helpers;
using modern_pomo.Interfaces;
using modern_pomo.Services;
using modern_pomo.Views;

namespace modern_pomo.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IPomodoroTimerService _pomodoroTimerService;
    
    [ObservableProperty]
    private string _remainingTimeDisplay = "01:00";

    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(CanStartPomodoro))]
    [NotifyPropertyChangedFor(nameof(CanStopPomodoro))]
    private bool _isPomodoroSessionRunning = false;
    
    public bool CanStartPomodoro => !IsPomodoroSessionRunning;
    public bool CanStopPomodoro => IsPomodoroSessionRunning;

    public State FocusModeState => State.Focus;
    public State BreakModeState => State.Break;
    
    public MainViewModel(IPomodoroTimerService pomodoroTimerService)
    {
           _pomodoroTimerService = pomodoroTimerService;
           _remainingTimeDisplay = TimeFormatter.FormatTimespan(pomodoroTimerService.GetSessionFocusDuration());
           
           _pomodoroTimerService.OnTimerDown += PomodoroTimerServiceOnOnTimerDown;
           _pomodoroTimerService.OnPomodoroSessionChange += PomodoroTimerServiceOnOnPomodoroSessionChange;
    }

    private void PomodoroTimerServiceOnOnPomodoroSessionChange(object? sender, PomodoroSessionChangeArgs e)
    {
        IsPomodoroSessionRunning = e.IsRunning;
    }

    private void PomodoroTimerServiceOnOnTimerDown(object? sender, TimerEventArgs e)
    {
        RemainingTimeDisplay = TimeFormatter.FormatTimespan(e.RemainingTime);
    }

    [RelayCommand]
    private void StartPomodoroTimer()
    {
        _pomodoroTimerService.StartSession();
    }
    
    [RelayCommand]
    private void StopPomodoroTimer()
    {
        _pomodoroTimerService.StopSession();
    }

    [RelayCommand]
    private void RestartPomodoroTimer()
    {
        _pomodoroTimerService.RestartSession();
    }

    [RelayCommand]
    private void AddFocusTime()
    {
        _pomodoroTimerService.AddFocusTime(TimeSpan.FromMinutes(1));
    }

    [RelayCommand]
    private void RemoveFocusTime()
    {
        _pomodoroTimerService.RemoveFocusTime(TimeSpan.FromMinutes(1));
    }

    [RelayCommand]
    private void OpenSettingsWindow()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var settingsWindow = new SettingsWindow
            {
                DataContext = App.ServiceProvider.GetRequiredService<SettingsViewModel>()
            };
            settingsWindow.Show();
        });
    }

    [RelayCommand]
    private void SetCurrentState(State state)
    {
        _pomodoroTimerService.SetCurrentState(state);
    }
}