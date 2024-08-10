using System.Runtime.InteropServices;

using FFXIVClientStructs.Attributes;
using FFXIVClientStructs.FFXIV.Component.GUI;

#pragma warning disable SA1134 // Attributes should not share line
namespace AutoTravel.GameSystems.Addons;

[Addon("_CharaSelectListMenu")]
[StructLayout(LayoutKind.Explicit, Size = 0x278)]
public unsafe partial struct AddonCharaSelectListMenu
{
    [FieldOffset(0)] public AtkUnitBase AtkUnitBase;

    [FieldOffset(0x238)] public AtkComponentButton* NewCharacterButton;
    [FieldOffset(0x240)] public AtkComponentButton* WorldButton;
    [FieldOffset(0x248)] public AtkComponentList* CharaList;                // list of characters
    [FieldOffset(0x250)] public AtkTextNode* WorldTextNode;
    [FieldOffset(0x258)] public AtkTextNode* ConnectionQualityTextNode;     // actual connection quality value
    [FieldOffset(0x260)] public AtkTextNode* CharaCountTextNode;            // node with 1/8 text
    [FieldOffset(0x270)] public AtkComponentButton* BackupUiClientButton;
}
