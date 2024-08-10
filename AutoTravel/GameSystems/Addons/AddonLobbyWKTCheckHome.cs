using System.Runtime.InteropServices;

using FFXIVClientStructs.Attributes;
using FFXIVClientStructs.FFXIV.Component.GUI;

#pragma warning disable SA1134 // Attributes should not share line
namespace AutoTravel.GameSystems.Addons;

[Addon("LobbyWKTCheckHome")]
[StructLayout(LayoutKind.Explicit, Size = 0x240)]
internal unsafe partial struct AddonLobbyWKTCheckHome
{
    [FieldOffset(0)] public AtkUnitBase AtkUnitBase;

    [FieldOffset(0x230)] public AtkComponentButton* ProcceedButton;
    [FieldOffset(0x238)] public AtkComponentButton* CancelButton;
}
