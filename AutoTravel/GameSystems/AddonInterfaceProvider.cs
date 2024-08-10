using Dalamud.Plugin.Services;

namespace AutoTravel.GameSystems;

internal class AddonInterfaceProvider(IAddonLifecycle addonLifecycle, IAddonEventManager eventManager)
{
    private readonly IAddonLifecycle addonLifecycle = addonLifecycle;
    private readonly IAddonEventManager eventManager = eventManager;

    internal AddonInterface<TAddon> AddonInterface<TAddon>(string name)
    {
        return new AddonInterface<TAddon>(this.addonLifecycle, this.eventManager, name);
    }
}
