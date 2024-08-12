using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoTravel.Utils;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace AutoTravel.GameSystems.Steps;

internal unsafe class ConfirmLogoutStep : BaseStep
{
    private readonly AddonInterface<AddonSelectYesno> addon;

    private bool running;
    private bool clickedOk;

    public ConfirmLogoutStep(AddonInterfaceProvider addonInterfaceProvider, EventLoop eventLoop, IStepActions actions)
        : base(eventLoop, actions)
    {
        this.addon = addonInterfaceProvider.AddonInterface<AddonSelectYesno>("SelectYesno");
        this.addon.AddHandler(AddonEvent.PostUpdate, this.Handle);
        this.addon.AddHandler(AddonEvent.PreFinalize, this.HandleFinalize);
    }

    public override void Dispose()
    {
        this.addon.Dispose();
    }

    protected override void RunInternal()
    {
        Plugin.Log.Info("Runing confirm logout step");
        this.running = true;
        this.clickedOk = false;
    }

    protected override void CancelInternal()
    {
        this.running = false;
    }

    private void Handle(AddonSelectYesno* addon)
    {
        if (this.running && !this.clickedOk && addon->YesButton != null)
        {
            Plugin.Log.Info("Clicking yes button");
            ClickAddonButton(addon->YesButton, &addon->AtkUnitBase);
            this.clickedOk = true;
        }
    }

    private void HandleFinalize(AddonSelectYesno* addon)
    {
        if (this.running)
        {
            var success = true;
            if (addon->AtkTextNode298 != null && addon->AtkTextNode298->IsVisible())
            {
                var text = addon->AtkTextNode298->NodeText.ToString();
                Plugin.Log.Info($"YesNo additional text PreFinalize: {text}");
                success = text == "0/20";
            }

            if (success)
            {
                Plugin.Log.Info("YesNo window closed, step completed");
                this.Success();
            }
            else
            {
                this.Fail(StepFailure.LogoutCanceled);
            }

            this.running = false;
        }
    }

    private static void ClickAddonButton(AtkComponentButton* target, AtkUnitBase* addon)
    {
        var btnRes = target->AtkComponentBase.OwnerNode->AtkResNode;
        var evt = btnRes.AtkEventManager.Event;
        addon->ReceiveEvent(evt->Type, (int)evt->Param, btnRes.AtkEventManager.Event);
    }
}
