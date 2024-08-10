using AutoTravel.GameSystems.Addons;
using AutoTravel.Utils;
using Dalamud.Game.Addon.Lifecycle;

namespace AutoTravel.GameSystems.Steps;

internal unsafe class SelectWorldStep : BaseStep
{
    private readonly AddonInterface<AddonLobbyDKTWorldList> addon;
    private readonly object sync = new();

    private string? targetDcName;
    private uint targetDcId;
    private string? targetWorld;
    private int step = -1;
    private int ticks;
    private bool running;

    internal SelectWorldStep(AddonInterfaceProvider addonInterfaceProvider, EventLoop eventLoop, IStepActions actions)
        : base(eventLoop, actions)
    {
        this.addon = addonInterfaceProvider.AddonInterface<AddonLobbyDKTWorldList>("LobbyDKTWorldList");
        this.addon.AddHandler(AddonEvent.PostUpdate, this.Handle);
    }

    public override void Dispose()
    {
        this.addon.Dispose();
    }

    internal void SelectReturnHome()
    {
        this.targetDcName = null;
    }

    internal void Select(string dcName, uint dcId, string? targetWorld)
    {
        this.targetDcName = dcName;
        this.targetDcId = dcId;
        this.targetWorld = targetWorld;
    }

    protected override void RunInternal()
    {
        Plugin.Log.Info("Running select world step");
        this.ticks = 10;
        this.step = this.targetDcName == null ? 2 : 0;
        this.running = true;
    }

    protected override void CancelInternal()
    {
        this.step = -1;
        this.running = false;
    }

    private void Handle(AddonLobbyDKTWorldList* addon)
    {
        if (!this.running)
        {
            return;
        }

        switch (this.step)
        {
            case 0:
                if (addon->SelectDataCenterList != null && addon->SelectDataCenterList->GetItemCount() > 0)
                {
                    var count = addon->SelectDataCenterList->GetItemCount();
                    var selectedDcIndex = -1;
                    for (var i = 0; i < count; i++)
                    {
                        var item = addon->SelectDataCenterList->GetItem(i);
                        var text = item->Renderer->ButtonTextNode->NodeText.ToString();
                        if (text == this.targetDcName)
                        {
                            selectedDcIndex = i;
                            break;
                        }
                    }

                    if (selectedDcIndex == -1)
                    {
                        this.step = -1;
                        this.Fail($"Cannot travel to selected Data Center: {this.targetDcName}\nData Center unavilable as a travel destination");
                    }
                    else
                    {
                        this.step++;
                        addon->SelectDataCenterList->SelectItem(selectedDcIndex, true);
                        Callbacks.Fire(&addon->AtkUnitBase, true, 1, 0, 0, this.targetDcId, false);
                    }

                    return;
                }

                break;
            case 1:
                if (addon->SelectWorldList != null && addon->SelectWorldList->GetItemCount() > 0)
                {
                    var count = addon->SelectWorldList->GetItemCount();
                    var selectedDirectly = -1;
                    var firstAvailable = -1;

                    for (var i = 1; i < count; i++)
                    {
                        var item = addon->SelectWorldList->GetItem(i);
                        var text = item->Renderer->ButtonTextNode->NodeText.ToString();
                        if (item->Renderer->IsEnabled)
                        {
                            if (firstAvailable == -1)
                            {
                                firstAvailable = i;
                            }

                            if (text == this.targetWorld)
                            {
                                selectedDirectly = i;
                                break;
                            }
                        }
                    }

                    if (selectedDirectly == -1)
                    {
                        selectedDirectly = firstAvailable;
                    }

                    if (selectedDirectly == -1)
                    {
                        this.step = -1;
                        this.Fail($"No worlds available in selected Data Center: {this.targetDcName}");
                    }
                    else
                    {
                        this.step++;
                        addon->SelectWorldList->SelectItem(selectedDirectly, true);
                        Callbacks.Fire(&addon->AtkUnitBase, true, 2, 0, selectedDirectly - 1, 0, 0);
                    }
                }

                break;
            case 2:
                if (addon->CancelButton != null && addon->CancelButton->IsEnabled && this.ticks-- < 0)
                {
                    this.step = -1;
                    this.running = false;
                    Plugin.Log.Info("Confirming Select World");
                    Callbacks.Fire(&addon->AtkUnitBase, true, 4);
                    this.Success();
                }

                break;
        }
    }
}
