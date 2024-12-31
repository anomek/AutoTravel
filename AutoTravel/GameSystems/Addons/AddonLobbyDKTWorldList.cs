using System.Runtime.InteropServices;

using FFXIVClientStructs.Attributes;
using FFXIVClientStructs.FFXIV.Component.GUI;

#pragma warning disable SA1134 // Attributes should not share line
namespace AutoTravel.GameSystems.Addons;

[Addon("LobbyDKTWorldList")]
[StructLayout(LayoutKind.Explicit, Size = 0x2d0)]
public unsafe partial struct AddonLobbyDKTWorldList
{
    [FieldOffset(0)] public AtkUnitBase AtkUnitBase;

    [FieldOffset(0x250)] public AtkComponentTreeList* SelectDataCenterList;
    [FieldOffset(0x290)] public AtkComponentTreeList* SelectWorldList;

    [FieldOffset(0x2c8)] public AtkComponentButton* ConfirmButton;
    [FieldOffset(0x2d0)] public AtkComponentButton* CancelButton;
}
