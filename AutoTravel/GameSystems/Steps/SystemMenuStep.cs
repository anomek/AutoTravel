using AutoTravel.Utils;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace AutoTravel.GameSystems.Steps;

internal unsafe class SystemMenuStep(IGameGui gameGui, EventLoop eventLoop, IStepActions actions)
    : BaseStep(eventLoop, actions)
{
    private readonly IGameGui gameGui = gameGui;

    public override void Dispose()
    {
    }

    protected override void RunInternal()
    {
        Plugin.Log.Info("Runing logout step");
        var addonAddr = this.gameGui.GetAddonByName("_MainCommand");
        if (addonAddr == 0)
        {
            this.Fail("Can't log out from game. Log out manually and travel from character selection menu");
        }
        else
        {
#pragma warning disable SA1117 // Parameters should be on same line or separate lines
            Callbacks.Fire((AtkUnitBase*)addonAddr, true,
                2, 1,
                "System", 3742, 2006,
                12,
                32, 90, 68, 30, 34, 19, 22, 21, 20, 37, 23, 24,
                Callbacks.ZeroAtkValue, Callbacks.ZeroAtkValue, Callbacks.ZeroAtkValue, Callbacks.ZeroAtkValue, Callbacks.ZeroAtkValue,
                Callbacks.ZeroAtkValue, Callbacks.ZeroAtkValue, Callbacks.ZeroAtkValue, Callbacks.ZeroAtkValue, Callbacks.ZeroAtkValue,
                Callbacks.ZeroAtkValue, Callbacks.ZeroAtkValue, Callbacks.ZeroAtkValue, Callbacks.ZeroAtkValue, Callbacks.ZeroAtkValue,
                Callbacks.ZeroAtkValue, Callbacks.ZeroAtkValue, Callbacks.ZeroAtkValue, Callbacks.ZeroAtkValue, Callbacks.ZeroAtkValue,
                Callbacks.ZeroAtkValue, Callbacks.ZeroAtkValue, Callbacks.ZeroAtkValue, Callbacks.ZeroAtkValue, Callbacks.ZeroAtkValue);
#pragma warning restore SA1117 // Parameters should be on same line or separate lines
            this.Success();
        }
    }

    protected override void CancelInternal()
    {
    }
}
