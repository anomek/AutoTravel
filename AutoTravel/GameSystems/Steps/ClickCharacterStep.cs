using AutoTravel.GameSystems.Addons;
using AutoTravel.Utils;
using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace AutoTravel.GameSystems.Steps;

internal unsafe class ClickCharacterStep : BaseStep
{
    private readonly AddonInterface<AddonCharaSelectListMenu> addon;

    private int index;
    private bool rightClick;
    private bool running;

    internal ClickCharacterStep(AddonInterfaceProvider provider, EventLoop eventLoop, IStepActions actions)
        : base(eventLoop, actions)
    {
        this.addon = provider.AddonInterface<AddonCharaSelectListMenu>("_CharaSelectListMenu");
        this.addon.AddHandler(AddonEvent.PostUpdate, this.Handle);
    }

    public override void Dispose()
    {
        this.addon.Dispose();
    }

    internal void SetClick(int index, bool rightClick)
    {
        this.index = index;
        this.rightClick = rightClick;
    }

    protected override void RunInternal()
    {
        Plugin.Log.Info("Runing click character step");
        this.running = true;
    }

    protected override void CancelInternal()
    {
        this.running = false;
    }

    private void Handle(AddonCharaSelectListMenu* addon)
    {
        if (this.running)
        {
            var length = addon->CharaList->GetItemCount();
            if (length > 0 && this.index < length)
            {
                var eventIndex = (byte)(5 + this.index);
                var evt = stackalloc AtkEvent[] { this.CreateAtkEvent(addon->AtkUnitBase, eventIndex) };
                var data = stackalloc AtkEventData[] { new AtkEventDataBuilder().Write<byte>(6, this.rightClick ? (byte)1 : (byte)0).Build() };
                addon->AtkUnitBase.ReceiveEvent(AtkEventType.MouseClick, eventIndex, evt, data);

                this.running = false;
                this.Success();
            }
        }
    }

    private unsafe class AtkEventDataBuilder
    {
        private readonly AtkEventData data;

        public AtkEventDataBuilder()
        {
#pragma warning disable SA1129 // Do not use default value type constructor
            this.data = new();
#pragma warning restore SA1129 // Do not use default value type constructor
        }

        public AtkEventDataBuilder Write<T>(int pos, T data)
            where T : unmanaged
        {
            fixed (AtkEventData* eventDataPtr = &this.data)
            {
                var ptr = ((nint)eventDataPtr) + pos;
                *(T*)ptr = data;
            }

            return this;
        }

        public AtkEventData Build() => this.data;
    }

    private AtkEvent CreateAtkEvent(AtkUnitBase atkUnitBase, byte flags = 0)
    {
        return new()
        {
            Listener = &atkUnitBase.AtkEventListener,
            Flags = flags,
            Target = &AtkStage.Instance()->AtkEventTarget,
        };
    }
}
