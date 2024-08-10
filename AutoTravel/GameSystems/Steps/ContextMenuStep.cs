using AutoTravel.Utils;
using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace AutoTravel.GameSystems.Steps;

internal unsafe class ContextMenuStep : BaseStep
{
    private readonly AddonInterface<AddonContextMenu> addon;

    private bool running;
    private int index;

    internal ContextMenuStep(AddonInterfaceProvider addonInterfaceProvider, EventLoop eventLoop, IStepActions actions)
        : base(eventLoop, actions)
    {
        this.addon = addonInterfaceProvider.AddonInterface<AddonContextMenu>("ContextMenu");
        this.addon.AddHandler(AddonEvent.PostUpdate, this.Handle);
    }

    public override void Dispose()
    {
        this.addon.Dispose();
    }

    internal void SetItem(int index)
    {
        this.index = index;
    }

    protected override void RunInternal()
    {
        Plugin.Log.Info("Runing context menu step");
        this.running = true;
    }

    protected override void CancelInternal()
    {
        this.running = false;
    }

    private void Handle(AddonContextMenu* addon)
    {
        if (this.running)
        {
            if (addon->OnMenuSelected(this.index, 0))
            {
                this.running = false;
                this.Success();
            }
        }
    }
}
