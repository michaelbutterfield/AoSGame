using Godot;
using System.Collections.Generic;

public partial class GameSettings : Node
{
    public static GameSettings Instance { get; private set; }
    
    [Signal]
    public delegate void SettingsChangedEventHandler();
    
    // Graphics Settings
    [Export] public int ResolutionWidth = 1920;
    [Export] public int ResolutionHeight = 1080;
    [Export] public bool Fullscreen = false;
    [Export] public bool VSync = true;
    [Export] public int MaxFPS = 60;
    [Export] public float CameraSensitivity = 1.0f;
    [Export] public float CameraZoomSpeed = 1.0f;
    [Export] public bool ShowFPS = false;
    [Export] public bool ShowUnitInfo = true;
    [Export] public bool ShowMeasurementLines = true;
    [Export] public bool ShowTerrainEffects = true;
    
    // Audio Settings
    [Export] public float MasterVolume = 1.0f;
    [Export] public float MusicVolume = 0.7f;
    [Export] public float SFXVolume = 0.8f;
    [Export] public bool EnableAudio = true;
    [Export] public bool EnableDiceSounds = true;
    [Export] public bool EnableUnitSounds = true;
    
    // Game Rules Settings
    [Export] public int PointLimit = 2000;
    [Export] public int MaxTurns = 5;
    [Export] public bool EnableTerrainEffects = true;
    [Export] public bool EnableMagicSystem = true;
    [Export] public bool EnablePrayerSystem = true;
    [Export] public bool EnableCommandAbilities = true;
    [Export] public bool EnableRallyPhase = true;
    [Export] public bool EnableVictoryConditions = true;
    [Export] public bool EnableObjectives = true;
    [Export] public bool EnableRandomEvents = false;
    
    // Multiplayer Settings
    [Export] public string DefaultServerAddress = "127.0.0.1";
    [Export] public int DefaultPort = 7777;
    [Export] public int MaxPlayers = 2;
    [Export] public bool EnableChat = true;
    [Export] public bool EnableSpectators = false;
    [Export] public float TurnTimeLimit = 300.0f; // 5 minutes
    [Export] public bool AutoAdvanceTurns = false;
    
    // AI Settings
    [Export] public bool EnableAI = true;
    [Export] public float AIDecisionDelay = 1.0f;
    [Export] public int AIDifficulty = 1; // 1=Easy, 2=Normal, 3=Hard
    [Export] public bool AIUsesAdvancedTactics = false;
    [Export] public bool AIUsesMagic = true;
    [Export] public bool AIUsesPrayers = true;
    
    // Input Settings
    [Export] public bool InvertMouseY = false;
    [Export] public bool InvertMouseX = false;
    [Export] public float MouseSensitivity = 1.0f;
    [Export] public bool EnableMouseAcceleration = false;
    [Export] public bool EnableGamepad = true;
    [Export] public float GamepadSensitivity = 1.0f;
    
    // UI Settings
    [Export] public float UIScale = 1.0f;
    [Export] public bool EnableTooltips = true;
    [Export] public bool EnableNotifications = true;
    [Export] public bool EnableMinimap = true;
    [Export] public bool EnableUnitHealthBars = true;
    [Export] public bool EnableTurnTimer = true;
    [Export] public string Language = "en";
    [Export] public bool EnableAccessibility = false;
    
    // Performance Settings
    [Export] public int ShadowQuality = 2; // 0=Off, 1=Low, 2=Medium, 3=High
    [Export] public int TextureQuality = 2;
    [Export] public int ModelQuality = 2;
    [Export] public bool EnableParticles = true;
    [Export] public bool EnablePostProcessing = true;
    [Export] public bool EnableAntiAliasing = true;
    [Export] public int AntiAliasingLevel = 2; // 0=Off, 1=FXAA, 2=MSAAx2, 3=MSAAx4
    
    // Advanced Settings
    [Export] public bool EnableDebugMode = false;
    [Export] public bool EnableConsole = false;
    [Export] public bool EnableProfiling = false;
    [Export] public bool EnableAutoSave = true;
    [Export] public int AutoSaveInterval = 300; // 5 minutes
    [Export] public bool EnableCrashReporting = true;
    
    private const string SETTINGS_FILE = "user://settings.cfg";
    private ConfigFile _configFile;
    
    public override void _Ready()
    {
        Instance = this;
        _configFile = new ConfigFile();
        LoadSettings();
        ApplySettings();
    }
    
    public void LoadSettings()
    {
        var error = _configFile.Load(SETTINGS_FILE);
        if (error != Error.Ok)
        {
            GD.Print("No settings file found, using defaults");
            return;
        }
        
        // Graphics
        ResolutionWidth = _configFile.GetValue("Graphics", "ResolutionWidth", ResolutionWidth);
        ResolutionHeight = _configFile.GetValue("Graphics", "ResolutionHeight", ResolutionHeight);
        Fullscreen = _configFile.GetValue("Graphics", "Fullscreen", Fullscreen);
        VSync = _configFile.GetValue("Graphics", "VSync", VSync);
        MaxFPS = _configFile.GetValue("Graphics", "MaxFPS", MaxFPS);
        CameraSensitivity = _configFile.GetValue("Graphics", "CameraSensitivity", CameraSensitivity);
        CameraZoomSpeed = _configFile.GetValue("Graphics", "CameraZoomSpeed", CameraZoomSpeed);
        ShowFPS = _configFile.GetValue("Graphics", "ShowFPS", ShowFPS);
        ShowUnitInfo = _configFile.GetValue("Graphics", "ShowUnitInfo", ShowUnitInfo);
        ShowMeasurementLines = _configFile.GetValue("Graphics", "ShowMeasurementLines", ShowMeasurementLines);
        ShowTerrainEffects = _configFile.GetValue("Graphics", "ShowTerrainEffects", ShowTerrainEffects);
        
        // Audio
        MasterVolume = _configFile.GetValue("Audio", "MasterVolume", MasterVolume);
        MusicVolume = _configFile.GetValue("Audio", "MusicVolume", MusicVolume);
        SFXVolume = _configFile.GetValue("Audio", "SFXVolume", SFXVolume);
        EnableAudio = _configFile.GetValue("Audio", "EnableAudio", EnableAudio);
        EnableDiceSounds = _configFile.GetValue("Audio", "EnableDiceSounds", EnableDiceSounds);
        EnableUnitSounds = _configFile.GetValue("Audio", "EnableUnitSounds", EnableUnitSounds);
        
        // Game Rules
        PointLimit = _configFile.GetValue("GameRules", "PointLimit", PointLimit);
        MaxTurns = _configFile.GetValue("GameRules", "MaxTurns", MaxTurns);
        EnableTerrainEffects = _configFile.GetValue("GameRules", "EnableTerrainEffects", EnableTerrainEffects);
        EnableMagicSystem = _configFile.GetValue("GameRules", "EnableMagicSystem", EnableMagicSystem);
        EnablePrayerSystem = _configFile.GetValue("GameRules", "EnablePrayerSystem", EnablePrayerSystem);
        EnableCommandAbilities = _configFile.GetValue("GameRules", "EnableCommandAbilities", EnableCommandAbilities);
        EnableRallyPhase = _configFile.GetValue("GameRules", "EnableRallyPhase", EnableRallyPhase);
        EnableVictoryConditions = _configFile.GetValue("GameRules", "EnableVictoryConditions", EnableVictoryConditions);
        EnableObjectives = _configFile.GetValue("GameRules", "EnableObjectives", EnableObjectives);
        EnableRandomEvents = _configFile.GetValue("GameRules", "EnableRandomEvents", EnableRandomEvents);
        
        // Multiplayer
        DefaultServerAddress = _configFile.GetValue("Multiplayer", "DefaultServerAddress", DefaultServerAddress);
        DefaultPort = _configFile.GetValue("Multiplayer", "DefaultPort", DefaultPort);
        MaxPlayers = _configFile.GetValue("Multiplayer", "MaxPlayers", MaxPlayers);
        EnableChat = _configFile.GetValue("Multiplayer", "EnableChat", EnableChat);
        EnableSpectators = _configFile.GetValue("Multiplayer", "EnableSpectators", EnableSpectators);
        TurnTimeLimit = _configFile.GetValue("Multiplayer", "TurnTimeLimit", TurnTimeLimit);
        AutoAdvanceTurns = _configFile.GetValue("Multiplayer", "AutoAdvanceTurns", AutoAdvanceTurns);
        
        // AI
        EnableAI = _configFile.GetValue("AI", "EnableAI", EnableAI);
        AIDecisionDelay = _configFile.GetValue("AI", "AIDecisionDelay", AIDecisionDelay);
        AIDifficulty = _configFile.GetValue("AI", "AIDifficulty", AIDifficulty);
        AIUsesAdvancedTactics = _configFile.GetValue("AI", "AIUsesAdvancedTactics", AIUsesAdvancedTactics);
        AIUsesMagic = _configFile.GetValue("AI", "AIUsesMagic", AIUsesMagic);
        AIUsesPrayers = _configFile.GetValue("AI", "AIUsesPrayers", AIUsesPrayers);
        
        // Input
        InvertMouseY = _configFile.GetValue("Input", "InvertMouseY", InvertMouseY);
        InvertMouseX = _configFile.GetValue("Input", "InvertMouseX", InvertMouseX);
        MouseSensitivity = _configFile.GetValue("Input", "MouseSensitivity", MouseSensitivity);
        EnableMouseAcceleration = _configFile.GetValue("Input", "EnableMouseAcceleration", EnableMouseAcceleration);
        EnableGamepad = _configFile.GetValue("Input", "EnableGamepad", EnableGamepad);
        GamepadSensitivity = _configFile.GetValue("Input", "GamepadSensitivity", GamepadSensitivity);
        
        // UI
        UIScale = _configFile.GetValue("UI", "UIScale", UIScale);
        EnableTooltips = _configFile.GetValue("UI", "EnableTooltips", EnableTooltips);
        EnableNotifications = _configFile.GetValue("UI", "EnableNotifications", EnableNotifications);
        EnableMinimap = _configFile.GetValue("UI", "EnableMinimap", EnableMinimap);
        EnableUnitHealthBars = _configFile.GetValue("UI", "EnableUnitHealthBars", EnableUnitHealthBars);
        EnableTurnTimer = _configFile.GetValue("UI", "EnableTurnTimer", EnableTurnTimer);
        Language = _configFile.GetValue("UI", "Language", Language);
        EnableAccessibility = _configFile.GetValue("UI", "EnableAccessibility", EnableAccessibility);
        
        // Performance
        ShadowQuality = _configFile.GetValue("Performance", "ShadowQuality", ShadowQuality);
        TextureQuality = _configFile.GetValue("Performance", "TextureQuality", TextureQuality);
        ModelQuality = _configFile.GetValue("Performance", "ModelQuality", ModelQuality);
        EnableParticles = _configFile.GetValue("Performance", "EnableParticles", EnableParticles);
        EnablePostProcessing = _configFile.GetValue("Performance", "EnablePostProcessing", EnablePostProcessing);
        EnableAntiAliasing = _configFile.GetValue("Performance", "EnableAntiAliasing", EnableAntiAliasing);
        AntiAliasingLevel = _configFile.GetValue("Performance", "AntiAliasingLevel", AntiAliasingLevel);
        
        // Advanced
        EnableDebugMode = _configFile.GetValue("Advanced", "EnableDebugMode", EnableDebugMode);
        EnableConsole = _configFile.GetValue("Advanced", "EnableConsole", EnableConsole);
        EnableProfiling = _configFile.GetValue("Advanced", "EnableProfiling", EnableProfiling);
        EnableAutoSave = _configFile.GetValue("Advanced", "EnableAutoSave", EnableAutoSave);
        AutoSaveInterval = _configFile.GetValue("Advanced", "AutoSaveInterval", AutoSaveInterval);
        EnableCrashReporting = _configFile.GetValue("Advanced", "EnableCrashReporting", EnableCrashReporting);
        
        GD.Print("Settings loaded successfully");
    }
    
    public void SaveSettings()
    {
        // Graphics
        _configFile.SetValue("Graphics", "ResolutionWidth", ResolutionWidth);
        _configFile.SetValue("Graphics", "ResolutionHeight", ResolutionHeight);
        _configFile.SetValue("Graphics", "Fullscreen", Fullscreen);
        _configFile.SetValue("Graphics", "VSync", VSync);
        _configFile.SetValue("Graphics", "MaxFPS", MaxFPS);
        _configFile.SetValue("Graphics", "CameraSensitivity", CameraSensitivity);
        _configFile.SetValue("Graphics", "CameraZoomSpeed", CameraZoomSpeed);
        _configFile.SetValue("Graphics", "ShowFPS", ShowFPS);
        _configFile.SetValue("Graphics", "ShowUnitInfo", ShowUnitInfo);
        _configFile.SetValue("Graphics", "ShowMeasurementLines", ShowMeasurementLines);
        _configFile.SetValue("Graphics", "ShowTerrainEffects", ShowTerrainEffects);
        
        // Audio
        _configFile.SetValue("Audio", "MasterVolume", MasterVolume);
        _configFile.SetValue("Audio", "MusicVolume", MusicVolume);
        _configFile.SetValue("Audio", "SFXVolume", SFXVolume);
        _configFile.SetValue("Audio", "EnableAudio", EnableAudio);
        _configFile.SetValue("Audio", "EnableDiceSounds", EnableDiceSounds);
        _configFile.SetValue("Audio", "EnableUnitSounds", EnableUnitSounds);
        
        // Game Rules
        _configFile.SetValue("GameRules", "PointLimit", PointLimit);
        _configFile.SetValue("GameRules", "MaxTurns", MaxTurns);
        _configFile.SetValue("GameRules", "EnableTerrainEffects", EnableTerrainEffects);
        _configFile.SetValue("GameRules", "EnableMagicSystem", EnableMagicSystem);
        _configFile.SetValue("GameRules", "EnablePrayerSystem", EnablePrayerSystem);
        _configFile.SetValue("GameRules", "EnableCommandAbilities", EnableCommandAbilities);
        _configFile.SetValue("GameRules", "EnableRallyPhase", EnableRallyPhase);
        _configFile.SetValue("GameRules", "EnableVictoryConditions", EnableVictoryConditions);
        _configFile.SetValue("GameRules", "EnableObjectives", EnableObjectives);
        _configFile.SetValue("GameRules", "EnableRandomEvents", EnableRandomEvents);
        
        // Multiplayer
        _configFile.SetValue("Multiplayer", "DefaultServerAddress", DefaultServerAddress);
        _configFile.SetValue("Multiplayer", "DefaultPort", DefaultPort);
        _configFile.SetValue("Multiplayer", "MaxPlayers", MaxPlayers);
        _configFile.SetValue("Multiplayer", "EnableChat", EnableChat);
        _configFile.SetValue("Multiplayer", "EnableSpectators", EnableSpectators);
        _configFile.SetValue("Multiplayer", "TurnTimeLimit", TurnTimeLimit);
        _configFile.SetValue("Multiplayer", "AutoAdvanceTurns", AutoAdvanceTurns);
        
        // AI
        _configFile.SetValue("AI", "EnableAI", EnableAI);
        _configFile.SetValue("AI", "AIDecisionDelay", AIDecisionDelay);
        _configFile.SetValue("AI", "AIDifficulty", AIDifficulty);
        _configFile.SetValue("AI", "AIUsesAdvancedTactics", AIUsesAdvancedTactics);
        _configFile.SetValue("AI", "AIUsesMagic", AIUsesMagic);
        _configFile.SetValue("AI", "AIUsesPrayers", AIUsesPrayers);
        
        // Input
        _configFile.SetValue("Input", "InvertMouseY", InvertMouseY);
        _configFile.SetValue("Input", "InvertMouseX", InvertMouseX);
        _configFile.SetValue("Input", "MouseSensitivity", MouseSensitivity);
        _configFile.SetValue("Input", "EnableMouseAcceleration", EnableMouseAcceleration);
        _configFile.SetValue("Input", "EnableGamepad", EnableGamepad);
        _configFile.SetValue("Input", "GamepadSensitivity", GamepadSensitivity);
        
        // UI
        _configFile.SetValue("UI", "UIScale", UIScale);
        _configFile.SetValue("UI", "EnableTooltips", EnableTooltips);
        _configFile.SetValue("UI", "EnableNotifications", EnableNotifications);
        _configFile.SetValue("UI", "EnableMinimap", EnableMinimap);
        _configFile.SetValue("UI", "EnableUnitHealthBars", EnableUnitHealthBars);
        _configFile.SetValue("UI", "EnableTurnTimer", EnableTurnTimer);
        _configFile.SetValue("UI", "Language", Language);
        _configFile.SetValue("UI", "EnableAccessibility", EnableAccessibility);
        
        // Performance
        _configFile.SetValue("Performance", "ShadowQuality", ShadowQuality);
        _configFile.SetValue("Performance", "TextureQuality", TextureQuality);
        _configFile.SetValue("Performance", "ModelQuality", ModelQuality);
        _configFile.SetValue("Performance", "EnableParticles", EnableParticles);
        _configFile.SetValue("Performance", "EnablePostProcessing", EnablePostProcessing);
        _configFile.SetValue("Performance", "EnableAntiAliasing", EnableAntiAliasing);
        _configFile.SetValue("Performance", "AntiAliasingLevel", AntiAliasingLevel);
        
        // Advanced
        _configFile.SetValue("Advanced", "EnableDebugMode", EnableDebugMode);
        _configFile.SetValue("Advanced", "EnableConsole", EnableConsole);
        _configFile.SetValue("Advanced", "EnableProfiling", EnableProfiling);
        _configFile.SetValue("Advanced", "EnableAutoSave", EnableAutoSave);
        _configFile.SetValue("Advanced", "AutoSaveInterval", AutoSaveInterval);
        _configFile.SetValue("Advanced", "EnableCrashReporting", EnableCrashReporting);
        
        var error = _configFile.Save(SETTINGS_FILE);
        if (error == Error.Ok)
        {
            GD.Print("Settings saved successfully");
        }
        else
        {
            GD.PrintErr($"Failed to save settings: {error}");
        }
    }
    
    public void ApplySettings()
    {
        // Apply graphics settings
        var window = GetWindow();
        if (window != null)
        {
            window.Size = new Vector2I(ResolutionWidth, ResolutionHeight);
            window.Mode = Fullscreen ? Window.ModeEnum.Fullscreen : Window.ModeEnum.Windowed;
            window.VSyncMode = VSync ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled;
        }
        
        // Apply audio settings
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), Mathf.LinearToDb(MasterVolume));
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), Mathf.LinearToDb(MusicVolume));
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"), Mathf.LinearToDb(SFXVolume));
        
        // Apply performance settings
        RenderingServer.SetDefaultClearColor(new Color(0.2f, 0.2f, 0.3f));
        
        // Apply UI settings
        GetViewport().GuiEmbedSubwindows = true;
        
        EmitSignal(SignalName.SettingsChanged);
        GD.Print("Settings applied successfully");
    }
    
    public void ResetToDefaults()
    {
        // Reset all settings to default values
        ResolutionWidth = 1920;
        ResolutionHeight = 1080;
        Fullscreen = false;
        VSync = true;
        MaxFPS = 60;
        CameraSensitivity = 1.0f;
        CameraZoomSpeed = 1.0f;
        ShowFPS = false;
        ShowUnitInfo = true;
        ShowMeasurementLines = true;
        ShowTerrainEffects = true;
        
        MasterVolume = 1.0f;
        MusicVolume = 0.7f;
        SFXVolume = 0.8f;
        EnableAudio = true;
        EnableDiceSounds = true;
        EnableUnitSounds = true;
        
        PointLimit = 2000;
        MaxTurns = 5;
        EnableTerrainEffects = true;
        EnableMagicSystem = true;
        EnablePrayerSystem = true;
        EnableCommandAbilities = true;
        EnableRallyPhase = true;
        EnableVictoryConditions = true;
        EnableObjectives = true;
        EnableRandomEvents = false;
        
        DefaultServerAddress = "127.0.0.1";
        DefaultPort = 7777;
        MaxPlayers = 2;
        EnableChat = true;
        EnableSpectators = false;
        TurnTimeLimit = 300.0f;
        AutoAdvanceTurns = false;
        
        EnableAI = true;
        AIDecisionDelay = 1.0f;
        AIDifficulty = 1;
        AIUsesAdvancedTactics = false;
        AIUsesMagic = true;
        AIUsesPrayers = true;
        
        InvertMouseY = false;
        InvertMouseX = false;
        MouseSensitivity = 1.0f;
        EnableMouseAcceleration = false;
        EnableGamepad = true;
        GamepadSensitivity = 1.0f;
        
        UIScale = 1.0f;
        EnableTooltips = true;
        EnableNotifications = true;
        EnableMinimap = true;
        EnableUnitHealthBars = true;
        EnableTurnTimer = true;
        Language = "en";
        EnableAccessibility = false;
        
        ShadowQuality = 2;
        TextureQuality = 2;
        ModelQuality = 2;
        EnableParticles = true;
        EnablePostProcessing = true;
        EnableAntiAliasing = true;
        AntiAliasingLevel = 2;
        
        EnableDebugMode = false;
        EnableConsole = false;
        EnableProfiling = false;
        EnableAutoSave = true;
        AutoSaveInterval = 300;
        EnableCrashReporting = true;
        
        ApplySettings();
        SaveSettings();
        GD.Print("Settings reset to defaults");
    }
    
    public Dictionary<string, object> GetSettingsSnapshot()
    {
        return new Dictionary<string, object>
        {
            {"ResolutionWidth", ResolutionWidth},
            {"ResolutionHeight", ResolutionHeight},
            {"Fullscreen", Fullscreen},
            {"VSync", VSync},
            {"MaxFPS", MaxFPS},
            {"MasterVolume", MasterVolume},
            {"MusicVolume", MusicVolume},
            {"SFXVolume", SFXVolume},
            {"PointLimit", PointLimit},
            {"MaxTurns", MaxTurns},
            {"EnableAI", EnableAI},
            {"AIDifficulty", AIDifficulty},
            {"Language", Language}
        };
    }
}
