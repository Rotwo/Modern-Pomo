using System;

namespace modern_pomo.Models;

public class PomodoroSessionModel(TimeSpan focusDuration, TimeSpan breakDuration, int maxCycles)
{
    public TimeSpan FocusDuration { get; set; } = focusDuration;
    public TimeSpan BreakDuration { get; set; } = breakDuration;
    public int CompletedCycles {get; private set;}
    public int MaxCycles {get; set;} = maxCycles;
    public bool IsRunning {get; private set;}

    public void Start()
    {
        IsRunning = true;
    }

    public void Stop()
    {
        IsRunning = false;
    }

    public void CompleteCycle()
    {
        CompletedCycles++;
    }
}