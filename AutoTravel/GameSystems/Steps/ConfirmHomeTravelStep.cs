using AutoTravel.GameSystems.Addons;
using AutoTravel.Utils;
using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace AutoTravel.GameSystems.Steps;

internal unsafe class ConfirmHomeTravelStep : BaseStep
{
    private readonly AddonInterface<AddonLobbyWKTCheckHome> addon;
    private readonly AddonInterface<AddonSelectOk> selectOkAddon;

    private bool running;

    internal ConfirmHomeTravelStep(AddonInterfaceProvider addonInterfaceProvider, EventLoop eventLoop, IStepActions actions)
        : base(eventLoop, actions)
    {
        this.addon = addonInterfaceProvider.AddonInterface<AddonLobbyWKTCheckHome>("LobbyWKTCheckHome");
        this.addon.AddHandler(AddonEvent.PostUpdate, this.Handle);
        this.selectOkAddon = addonInterfaceProvider.AddonInterface<AddonSelectOk>("SelectOk");
        this.selectOkAddon.AddHandler(AddonEvent.PostUpdate, this.HandleSelectOk);
    }

    public override void Dispose()
    {
        this.addon.Dispose();
        this.selectOkAddon.Dispose();
    }

    protected override void RunInternal()
    {
        this.running = true;
    }

    protected override void CancelInternal()
    {
        this.running = false;
    }

    private void Handle(AddonLobbyWKTCheckHome* addon)
    {
        if (this.running && addon->ProcceedButton != null && addon->ProcceedButton->IsEnabled)
        {
            this.running = false;
            Callbacks.Fire(&addon->AtkUnitBase, true, 0);
            this.Success();
        }
    }

    private void HandleSelectOk(AddonSelectOk* addon)
    {
        if (this.running && addon->AtkUnitBase.IsVisible)
        {
            Plugin.Log.Info("ConfirmHomeTravelStep: running=false (fail)");
            this.running = false;
            this.Fail(StepFailure.WorldTravelCooldown);
        }
    }
}
