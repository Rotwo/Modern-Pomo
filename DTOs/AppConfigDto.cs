using System;
using Newtonsoft.Json;

namespace modern_pomo.DTOs;

public class AppConfigDto
{
    [JsonProperty("focus_duration")]
    public TimeSpan FocusDuration = TimeSpan.FromMinutes(25);
    [JsonProperty("break_duration")]
    public TimeSpan BreakDuration = TimeSpan.FromMinutes(5);
    [JsonProperty("max_cycles")]
    public int MaxCycles = 1;
}