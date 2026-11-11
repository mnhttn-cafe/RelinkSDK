using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using Reloaded.Mod.Interfaces.Internal;
using RelinkSDK.Bindings;
using RelinkSDK.Configuration;
using RelinkSDK.Entities;
using RelinkSDK.Network;

namespace RelinkSDK;

public class RelinkStartup<TMod, TConfig> : IMod
    where TMod : RelinkMod<TConfig>, new()
    where TConfig : Configurable<TConfig>, new()
{
    private TMod? _mod;
    private Reloaded.Hooks.Definitions.IHook<AppMainLoop.StepFn>? _step;
    private bool _updateFailed;

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
        _mod.Network = new NetworkManager();
        _mod.Start();

        HandleStartup();
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

    private unsafe void HandleStartup()
    {
        var method = typeof(TMod).GetMethod(nameof(RelinkMod<TConfig>.Update), Type.EmptyTypes)!;

        if (method.GetBaseDefinition().DeclaringType
            != method.DeclaringType)
        {
            _step = AppMainLoop.HookStep(_mod!.Hooks, OnStep);
        }
    }

    private unsafe void OnStep(AppMainLoop* self)
    {
        _step!.OriginalFunction(self);

        if (_updateFailed)
        {
            return;
        }

        try
        {
            _mod!.Update();
        }
        catch (Exception e)
        {
            // better safe than sorry
            _updateFailed = true;
            _mod!.Log($"Update threw an exception, disabling it: {e}");
        }
    }
}
