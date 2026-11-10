using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using Reloaded.Mod.Interfaces.Internal;
using RelinkSDK.Configuration;
using RelinkSDK.Entities;

namespace RelinkSDK;

public class RelinkStartup<TMod, TConfig> : IMod
    where TMod : RelinkMod<TConfig>, new()
    where TConfig : Configurable<TConfig>, new()
{
    private TMod? _mod;

    public void StartEx(IModLoaderV1 loaderApi, IModConfigV1 modConfigApi)
    {
        var modLoader = (IModLoader)loaderApi;
        var modConfig = (IModConfig)modConfigApi;
        var logger = (ILogger)modLoader.GetLogger();

        if (modLoader.GetController<IReloadedHooks>()?.TryGetTarget(out var hooks) != true)
        {
            logger.WriteLine($"[{modConfig.ModId}] Reloaded.SharedLib.Hooks is missing, not loading");
            return;
        }

        if (!Game.Instance.IsSupported)
        {
            logger.WriteLine($"[{modConfig.ModId}] This game version isn't supported, not loading");
            return;
        }

        var configurator = new Configurator<TConfig>(modLoader.GetModConfigDirectory(modConfig.ModId));
        configurator.SetContext(new ConfiguratorContext { Application = modLoader.GetAppConfig() });
        var config = configurator.GetConfiguration<TConfig>(0);
        config.ConfigurationUpdated += OnConfigurationUpdated;

        _mod = new TMod
        {
            ModLoader = modLoader,
            ModConfig = modConfig,
            Hooks = hooks,
            Logger = logger,
            Config = config,
        };

        _mod.Entities = new EntityManager(hooks);

        _mod.Start();
    }

    private void OnConfigurationUpdated(IUpdatableConfigurable configurable)
    {
        if (_mod == null)
        {
            return;
        }

        _mod.Config = (TConfig)configurable;
        _mod.ConfigurationUpdated(_mod.Config);
    }

    public void Suspend()
    {
    }

    public void Resume()
    {
    }

    public void Unload()
    {
    }

    public bool CanUnload() => false;
    public bool CanSuspend() => false;
    public Action Disposing => () => { };
}
