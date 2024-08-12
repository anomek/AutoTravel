using AutoTravel.GameSystems.Addons;
using AutoTravel.Utils;
using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace AutoTravel.GameSystems.Steps;

internal unsafe class DKTExecStep : BaseStep
{
    private readonly AddonInterface<AddonLobbyDKTCheckExec> addon;
    private readonly AddonInterface<AddonSelectOk> selectOkAddon;

    private bool running;

    internal DKTExecStep(AddonInterfaceProvider addonInterfaceProvider, EventLoop eventLoop, IStepActions actions)
        : base(eventLoop, actions)
    {
        this.addon = addonInterfaceProvider.AddonInterface<AddonLobbyDKTCheckExec>("LobbyDKTCheckExec");
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

    private void Handle(AddonLobbyDKTCheckExec* addon)
    {
        if (this.running && addon->ProceedButton != null && addon->ProceedButton->IsEnabled)
        {
            Plugin.Log.Info("Firing addon");
            Callbacks.Fire(&addon->AtkUnitBase, true, 0);
            this.running = false;
            this.Success();
        }
    }

    private void HandleSelectOk(AddonSelectOk* addon)
    {
        if (this.running && addon->AtkUnitBase.IsVisible)
        {
            this.running = false;
            this.Fail(StepFailure.DKTExecGenericFailure);
        }
    }
}
