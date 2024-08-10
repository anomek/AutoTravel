using AutoTravel.GameSystems.Steps;
using AutoTravel.Utils;
using Dalamud.Plugin.Services;

namespace AutoTravel.GameSystems;

internal unsafe class TravelSteps(IGameGui gameGui, AddonInterfaceProvider addonInterfaceProvider, EventLoop eventLoop)
{
    private readonly IGameGui gameGui = gameGui;
    private readonly AddonInterfaceProvider addonInterfaceProvider = addonInterfaceProvider;

    internal EventLoop EventLoop { get; } = eventLoop;

    internal BaseStep SystemMenuStep(IStepActions actions)
    {
        return new SystemMenuStep(this.gameGui, this.EventLoop, actions);
    }

    internal BaseStep LogoutStep(IStepActions actions)
    {
        return new FireAddonStep(this.addonInterfaceProvider, "AddonContextMenuTitle", this.EventLoop, actions, 0, 10, 0u, 0, 0);
    }

    internal BaseStep ConfirmYesNo(IStepActions actions)
    {
        return new FireAddonStep(this.addonInterfaceProvider, "SelectYesno", this.EventLoop, actions, 0);
    }

    internal BaseStep StartStep(IStepActions actions)
    {
        return new FireAddonStep(this.addonInterfaceProvider, "_TitleMenu", this.EventLoop, actions, 4);
    }

    internal ClickCharacterStep ClickCharacterStep(IStepActions actions)
    {
        return new ClickCharacterStep(this.addonInterfaceProvider, this.EventLoop, actions);
    }

    internal ContextMenuStep ContextMenuStep(IStepActions actions)
    {
        return new ContextMenuStep(this.addonInterfaceProvider, this.EventLoop, actions);
    }

    internal BaseStep ConfirmDcTravel(IStepActions actions)
    {
        return new FireAddonStep(this.addonInterfaceProvider, "LobbyDKTCheck", this.EventLoop, actions, 0);
    }

    internal BaseStep ConfirmDcTravelExec(IStepActions actions)
    {
        return new DKTExecStep(this.addonInterfaceProvider, this.EventLoop, actions);
    }

    internal BaseStep SelectOkStep(IStepActions actions)
    {
        return new FireAddonStep(this.addonInterfaceProvider, "SelectOk", this.EventLoop, actions, 0);
    }

    internal SelectWorldStep SelectWorldStep(IStepActions actions)
    {
        return new SelectWorldStep(this.addonInterfaceProvider, this.EventLoop, actions);
    }

    internal BaseStep ConfirmHomeTravel(IStepActions actions)
    {
        return new ConfirmHomeTravelStep(this.addonInterfaceProvider, this.EventLoop, actions);
    }
}
