using System.Runtime.InteropServices;

using FFXIVClientStructs.Attributes;
using FFXIVClientStructs.FFXIV.Component.GUI;

#pragma warning disable SA1134 // Attributes should not share line
namespace AutoTravel.GameSystems.Addons;

[Addon("LobbyDKTCheckExec")]
[StructLayout(LayoutKind.Explicit, Size = 0x258)]
public unsafe partial struct AddonLobbyDKTCheckExec
{
    [FieldOffset(0)] public AtkUnitBase AtkUnitBase;

    [FieldOffset(0x240)] public AtkComponentButton* ProceedButton;
    [FieldOffset(0x250)] public AtkComponentButton* PreviousStepButton;
}
