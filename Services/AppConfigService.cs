using System;
using System.IO;
using System.Reflection;
using Avalonia;
using modern_pomo.DTOs;
using modern_pomo.Models;
using Newtonsoft.Json;

namespace modern_pomo.Services;

public class AppConfigService : IAppConfigService
{
    private const string ConfigFileName = "config.json";
    private const string ConfigFolderName = "config";
    private static readonly string? AppFolderPath = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
    private static readonly string ConfigFolder = Path.Combine(AppFolderPath ?? AppContext.BaseDirectory, ConfigFolderName);
    private static readonly string ConfigPath = Path.Combine(ConfigFolder, ConfigFileName);
    
    private AppConfigDto? _config;

    public AppConfigDto Config
    {
        get => _config ?? LoadConfig();
        set => _config = value;
    }

    public AppConfigDto LoadConfig()
    {
        Console.WriteLine("Loading config at: {0}", ConfigPath);
        if (!Directory.Exists(ConfigFolder))
            Directory.CreateDirectory(ConfigFolder);
        
        if(!File.Exists(ConfigPath)) return new AppConfigDto();
        
        var json = File.ReadAllText(ConfigPath);
        var appConfig = JsonConvert.DeserializeObject<AppConfigDto>(json);
        _config = appConfig ?? new AppConfigDto();
        return _config;
    }

    public void SaveConfig()
    {
        Console.WriteLine("Saving config at: {0}", ConfigPath);
        
        if(!Directory.Exists(ConfigFolder)) 
            Directory.CreateDirectory(ConfigFolder);
        
        var json = JsonConvert.SerializeObject(_config, Formatting.Indented);
        File.WriteAllText(ConfigPath, json);
    }
}