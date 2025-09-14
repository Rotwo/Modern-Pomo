using System;
using System.Timers;
using modern_pomo.Interfaces;
using modern_pomo.Models;

namespace modern_pomo.Services;

public enum State
{
    Focus,
    Break,
}

public class TimerEventArgs
{
    public TimeSpan RemainingTime { get; }

    public TimerEventArgs(TimeSpan remainingTime)
    {
        RemainingTime = remainingTime;
    }
}

public class PomodoroSessionChangeArgs
{
    public bool IsRunning { get; }

    public PomodoroSessionChangeArgs(bool isRunning)
    {
        IsRunning = isRunning;
    }
}

public class PomodoroTimerService : IPomodoroTimerService
{
    private readonly IAppConfigService _appConfigService;
    
    private System.Timers.Timer _timer;
    private PomodoroSessionModel _sessionModel;
    private State _state;
    
    private TimeSpan _remainingTime;

    public event EventHandler<TimerEventArgs> OnTimerDown = delegate { };
    public event EventHandler<PomodoroSessionChangeArgs> OnPomodoroSessionChange = delegate { };
    
    
    public PomodoroTimerService(IAppConfigService appConfigService)
    {
        _appConfigService = appConfigService;
        
        _timer = new System.Timers.Timer();
        _sessionModel = new PomodoroSessionModel(
            focusDuration: _appConfigService.Config.FocusDuration, 
            breakDuration: _appConfigService.Config.BreakDuration, 
            maxCycles: _appConfigService.Config.MaxCycles
        );
        
        // Configure timer
        _timer.Interval = 1000;
        _timer.Elapsed += OnTimerElapsed;
    }

    public TimeSpan GetSessionFocusDuration()
    {
        return _sessionModel.FocusDuration;
    }

    public TimeSpan GetSessionBreakDuration()
    {
        return _sessionModel.BreakDuration;
    }

    public void StartSession()
    {
        _state = State.Focus;
        _remainingTime = _sessionModel.FocusDuration;
        _timer.Start();
        _sessionModel.Start();
        OnPomodoroSessionChange.Invoke(this, new PomodoroSessionChangeArgs(true));
    }

    public void RestartSession()
    {
        _state = State.Focus;
        _remainingTime = _sessionModel.FocusDuration;
        StopSession();
        OnTimerDown.Invoke(this, new TimerEventArgs(_remainingTime));
    }

    public void ResumeSession()
    {
        _timer.Start();
        OnPomodoroSessionChange.Invoke(this, new PomodoroSessionChangeArgs(true));
    }

    public void StopSession()
    {
        _timer.Stop();
        _sessionModel.Stop();
        OnPomodoroSessionChange.Invoke(this, new PomodoroSessionChangeArgs(false));
    }
    
    public void PauseSession()
    {
        _timer.Stop();
        OnPomodoroSessionChange.Invoke(this, new PomodoroSessionChangeArgs(false));
    }

    public void AddFocusTime(TimeSpan time)
    {
        if(!_sessionModel.IsRunning) {
            _sessionModel.FocusDuration += time;
            OnTimerDown.Invoke(this, new TimerEventArgs(_sessionModel.FocusDuration));
        }
        else
        {
            _remainingTime += time;
            OnTimerDown.Invoke(this, new TimerEventArgs(_remainingTime));
        }
    }
    
    public void RemoveFocusTime(TimeSpan time)
    {
        if (!_sessionModel.IsRunning)
        {
            _sessionModel.FocusDuration -= time;
            OnTimerDown.Invoke(this, new TimerEventArgs(_sessionModel.FocusDuration));
        }
        else
        {
            _remainingTime -= time;
            OnTimerDown.Invoke(this, new TimerEventArgs(_remainingTime));
        }
    }

    public void SetFocusTime(TimeSpan time)
    {
        _sessionModel.FocusDuration = time;
        if(!_sessionModel.IsRunning)
            OnTimerDown.Invoke(this, new TimerEventArgs(time));
    }

    public void SetBreakTime(TimeSpan time)
    {
        _sessionModel.BreakDuration = time;
    }

    public void SetCycles(int cycles)
    {
        _sessionModel.MaxCycles = cycles;
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _remainingTime -= TimeSpan.FromSeconds(1);
        OnTimerDown.Invoke(this, new TimerEventArgs(_remainingTime));
        HandleState(_remainingTime);
    }

    private void HandleState(TimeSpan remainingTime)
    {
        if (remainingTime <= TimeSpan.Zero && _state == State.Focus)
        {
            Break();
        }
        else if (remainingTime <= TimeSpan.Zero && _state == State.Break)
        {
            Focus();
        }
    }

    private void Break()
    {
        Console.WriteLine("Starting Break");
        _state = State.Break;
        _remainingTime = _sessionModel.BreakDuration;
        _sessionModel.CompleteCycle();
    }

    private void Focus()
    {
        Console.WriteLine("Starting Focus");
        if (_sessionModel.CompletedCycles == _sessionModel.MaxCycles)
        {
            RestartSession();
            return;
        }
        _state = State.Focus;
        _remainingTime = _sessionModel.FocusDuration;
    }
}