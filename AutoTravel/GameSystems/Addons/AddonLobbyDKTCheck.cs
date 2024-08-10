using System.Runtime.InteropServices;

using FFXIVClientStructs.Attributes;
using FFXIVClientStructs.FFXIV.Component.GUI;

#pragma warning disable SA1134 // Attributes should not share line
namespace AutoTravel.GameSystems.Addons;

[Addon("LobbyDKTCheck")]
[StructLayout(LayoutKind.Explicit, Size = 0x248)]
public unsafe partial struct AddonLobbyDKTCheck
{
    [FieldOffset(0)] public AtkUnitBase AtkUnitBase;

    [FieldOffset(0x230)] public AtkComponentButton* HelpButton;
    [FieldOffset(0x238)] public AtkComponentButton* SelectButton;
    [FieldOffset(0x240)] public AtkComponentButton* CancelButton;
}
