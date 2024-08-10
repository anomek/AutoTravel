using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;

using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

#nullable disable
#pragma warning disable

namespace AutoTravel.GameSystems;

/*
 * source: https://github.com/NightmareXIV/ECommons/blob/master/ECommons/Automation/actions.cs
 */
public static unsafe class Callbacks
{
    private static readonly string Sig = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 44 24 ?? 0F B7 81";
    internal delegate byte AtkUnitBase_FireCallbacksDelegate(AtkUnitBase* Base, int valueCount, AtkValue* values, byte updateState);
    internal static AtkUnitBase_FireCallbacksDelegate Fireactions = null;
    private static Hook<AtkUnitBase_FireCallbacksDelegate> AtkUnitBase_FireactionsHook;


    private static IPluginLog PluginLog => Plugin.Log;

    public static readonly AtkValue ZeroAtkValue = new() { Type = 0, Int = 0 };

    internal static void Initialize()
    {
        var ptr = Plugin.SigScanner.ScanText(Sig);
        Fireactions = Marshal.GetDelegateForFunctionPointer<AtkUnitBase_FireCallbacksDelegate>(ptr);
        PluginLog.Information($"Initialized actions module, Fireactions = 0x{ptr:X16}");
    }


    public static void InstallHook()
    {
        if (Fireactions == null) Initialize();
        AtkUnitBase_FireactionsHook ??= Plugin.GameInterop.HookFromSignature<AtkUnitBase_FireCallbacksDelegate>(Sig, AtkUnitBase_FireactionsDetour);
        if (AtkUnitBase_FireactionsHook.IsEnabled)
        {
            PluginLog.Error("AtkUnitBase_FireactionsHook is already enabled");
        }
        else
        {
            AtkUnitBase_FireactionsHook.Enable();
            PluginLog.Information("AtkUnitBase_FireactionsHook enabled");
        }
    }

    public static void UninstallHook()
    {
        if (Fireactions == null)
        {
            PluginLog.Error("AtkUnitBase_FireactionsHook not initialized yet");
        }
        if (!AtkUnitBase_FireactionsHook.IsEnabled)
        {
            PluginLog.Error("AtkUnitBase_FireactionsHook is already disabled");
        }
        else
        {
            AtkUnitBase_FireactionsHook.Disable();
            PluginLog.Information("AtkUnitBase_FireactionsHook disabled");
        }
    }


    private static byte AtkUnitBase_FireactionsDetour(AtkUnitBase* Base, int valueCount, AtkValue* values, byte updateState)
    {
        try
        {
            var svalues = DecodeValues(valueCount, values).Select(x => $"    {x}");
            PluginLog.Info($"actions on {Base->NameString}, valueCount={valueCount}, updateState ={updateState}\n{string.Join("\n", svalues)}");
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "error");
        }

        var ret = AtkUnitBase_FireactionsHook?.Original(Base, valueCount, values, updateState);

        return ret ?? 0;
    }

    public static void FireRaw(AtkUnitBase* Base, int valueCount, AtkValue* values, byte updateState = 0)
    {
        if (Fireactions == null) Initialize();
        Fireactions(Base, valueCount, values, updateState);
    }

    public static void Fire(AtkUnitBase* Base, bool updateState, params object[] values)
    {
        if (Base == null) throw new Exception("Null UnitBase");
        var atkValues = (AtkValue*)Marshal.AllocHGlobal(values.Length * sizeof(AtkValue));
        if (atkValues == null) return;
        try
        {
            for (var i = 0; i < values.Length; i++)
            {
                var v = values[i];
                switch (v)
                {
                    case uint uintValue:
                        atkValues[i].Type = ValueType.UInt;
                        atkValues[i].UInt = uintValue;
                        break;
                    case int intValue:
                        atkValues[i].Type = ValueType.Int;
                        atkValues[i].Int = intValue;
                        break;
                    case float floatValue:
                        atkValues[i].Type = ValueType.Float;
                        atkValues[i].Float = floatValue;
                        break;
                    case bool boolValue:
                        atkValues[i].Type = ValueType.Bool;
                        atkValues[i].Byte = (byte)(boolValue ? 1 : 0);
                        break;
                    case string stringValue:
                        {
                            atkValues[i].Type = ValueType.String;
                            var stringBytes = Encoding.UTF8.GetBytes(stringValue);
                            var stringAlloc = Marshal.AllocHGlobal(stringBytes.Length + 1);
                            Marshal.Copy(stringBytes, 0, stringAlloc, stringBytes.Length);
                            Marshal.WriteByte(stringAlloc, stringBytes.Length, 0);
                            atkValues[i].String = (byte*)stringAlloc;
                            break;
                        }
                    case AtkValue rawValue:
                        {
                            atkValues[i] = rawValue;
                            break;
                        }
                    default:
                        throw new ArgumentException($"Unable to convert type {v.GetType()} to AtkValue");
                }
            }
            List<string> actionsValues = [];
            for (var i = 0; i < values.Length; i++)
            {
                actionsValues.Add($"    Value {i}: [input: {values[i]}/{values[i]?.GetType().Name}] -> {DecodeValue(atkValues[i])})");
            }
            //      PluginLog.Verbose($"Firing actions: {Base->Name.Read()}, valueCount = {values.Length}, updateStatte = {updateState}, values:\n");
            FireRaw(Base, values.Length, atkValues, (byte)(updateState ? 1 : 0));
        }
        finally
        {
            for (var i = 0; i < values.Length; i++)
            {
                if (atkValues[i].Type == ValueType.String)
                {
                    Marshal.FreeHGlobal(new nint(atkValues[i].String));
                }
            }
            Marshal.FreeHGlobal(new nint(atkValues));
        }
    }

    public static List<string> DecodeValues(int cnt, AtkValue* values)
    {
        var atkValueList = new List<string>();
        try
        {
            for (var i = 0; i < cnt; i++)
            {
                atkValueList.Add(DecodeValue(values[i]));
            }
        }
        catch (Exception e)
        {
            //     e.Log();
        }
        return atkValueList;
    }

    public static string DecodeValue(AtkValue a)
    {
        var str = new StringBuilder(a.Type.ToString()).Append(": ");
        switch (a.Type)
        {
            case ValueType.Int:
                {
                    str.Append(a.Int);
                    break;
                }
            case ValueType.String8:
            case ValueType.WideString:
            case ValueType.ManagedString:
            case ValueType.String:
                {
                    str.Append(Marshal.PtrToStringUTF8(new nint(a.String)));
                    break;
                }
            case ValueType.UInt:
                {
                    str.Append(a.UInt);
                    break;
                }
            case ValueType.Bool:
                {
                    str.Append(a.Byte != 0);
                    break;
                }
            default:
                {
                    str.Append($"Unknown Type: {a.Int}");
                    break;
                }
        }
        return str.ToString();
    }

    internal static void Dispose()
    {
        AtkUnitBase_FireactionsHook?.Dispose();
        AtkUnitBase_FireactionsHook = null;
        Fireactions = null;
    }
}
