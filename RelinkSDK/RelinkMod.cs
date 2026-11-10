using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using RelinkSDK.Configuration;
using RelinkSDK.Entities;

namespace RelinkSDK;

public abstract class RelinkMod<TConfig> where TConfig : Configurable<TConfig>, new()
{
    public IModLoader ModLoader { get; internal set; } = null!;
    public IModConfig ModConfig { get; internal set; } = null!;
    public IReloadedHooks Hooks { get; internal set; } = null!;
    public ILogger Logger { get; internal set; } = null!;
    public TConfig Config { get; internal set; } = null!;
    
    public EntityManager Entities { get; internal set; } = null!;

    public abstract void Start();

    public virtual void Update()
    {
        
    }

    public virtual void ConfigurationUpdated(TConfig config)
    {
    }

    public void Log(string message) => Logger.WriteLine($"[{ModConfig.ModId}] {message}");
}
