using modern_pomo.DTOs;

namespace modern_pomo.Models;

public interface IAppConfigService
{
    AppConfigDto LoadConfig();
    void SaveConfig();

    AppConfigDto Config
    {
        get;
        set;
    }
}