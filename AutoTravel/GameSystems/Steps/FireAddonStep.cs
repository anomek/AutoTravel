using AutoTravel.Utils;
using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace AutoTravel.GameSystems.Steps;

internal unsafe class FireAddonStep : BaseStep
{
    private readonly AddonInterface<AtkUnitBase> addon;
    private readonly object[] values;

    private bool running;

    internal FireAddonStep(AddonInterfaceProvider addonInterfaceProvider, string addonName, EventLoop eventLoop, IStepActions actions, params object[] values)
        : base(eventLoop, actions)
    {
        this.addon = addonInterfaceProvider.AddonInterface<AtkUnitBase>(addonName);
        this.addon.AddHandler(AddonEvent.PostUpdate, this.Handler);
        this.values = values;
    }

    public override void Dispose()
    {
        this.addon.Dispose();
    }

    protected override void RunInternal()
    {
        Plugin.Log.Info("Runing fire addon step");
        this.running = true;
    }

    protected override void CancelInternal()
    {
        this.running = false;
    }

    private void Handler(AtkUnitBase* addon)
    {
        if (this.running)
        {
            Plugin.Log.Info("Firing addon");
            Callbacks.Fire(addon, true, this.values);
            this.running = false;
            this.Success();
        }
    }
}
