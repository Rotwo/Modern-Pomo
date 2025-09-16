using System;
using modern_pomo.Services;

namespace modern_pomo.Interfaces;

public interface IPomodoroTimerService
{
    TimeSpan GetSessionFocusDuration();
    TimeSpan GetSessionBreakDuration();
    
    void StartSession();
    void StopSession();
    
    void RestartSession();
    
    void ResumeSession();
    void PauseSession();
    
    void AddFocusTime(TimeSpan time);
    void RemoveFocusTime(TimeSpan time);
    
    void SetFocusTime(TimeSpan time);
    void SetBreakTime(TimeSpan time);
    void SetCycles(int cycles);
    
    void SetCurrentState(State state);

    event EventHandler<TimerEventArgs> OnTimerDown;
    event EventHandler<PomodoroSessionChangeArgs> OnPomodoroSessionChange;
}