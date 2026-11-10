using Reloaded.Mod.Interfaces;

namespace RelinkSDK.Configuration;

/// <summary>
/// Creates the various different configurations used by the mod.
/// These configurations are available in the dropdown in Reloaded launcher.
/// </summary>
public class ConfiguratorMixinBase<TConfig> where TConfig : Configurable<TConfig>, new()
{
    /// <summary>
    /// Defines the configuration items to create.
    /// </summary>
    /// <param name="configFolder">Folder storing the configuration.</param>
    public virtual IUpdatableConfigurable[] MakeConfigurations(string configFolder)
    {
        return new IUpdatableConfigurable[]
        {
            Configurable<TConfig>.FromFile(Path.Combine(configFolder, "Config.json"), "Default Config")
        };
    }

    /// <summary>
    /// Allows for custom launcher/configurator implementation.
    /// If you have your own configuration program/code, run that code here and return true, else return false.
    /// </summary>
    public virtual bool TryRunCustomConfiguration(Configurator<TConfig> configurator)
    {
        return false;
    }

    /// <summary>
    /// Migrates from the old config location (usually mod folder) to the newer config location (separate folder).
    /// </summary>
    /// <param name="oldDirectory">Old directory containing the mod configs.</param>
    /// <param name="newDirectory">New directory pointing to user config folder.</param>
    public virtual void Migrate(string oldDirectory, string newDirectory)
    {
    }
}
