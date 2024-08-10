using System;
using System.Collections.Generic;
using System.Numerics;

using AutoTravel.GameSystems.Addons;
using AutoTravel.Travel;
using AutoTravel.Travel.WorldHelpers;
using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace AutoTravel.GameSystems;

internal unsafe class CharacterList : IDisposable
{
    internal delegate void OnCharacterListReady(CharacterListData data);

    private readonly AddonInterface<AddonCharaSelectListMenu> addon;
    private readonly List<OnCharacterListReady> onReady = [];

    private AddonCharaSelectListMenu* addonPtr;

    internal CharacterList(AddonInterfaceProvider addonInterfaceProvider)
    {
        this.addon = addonInterfaceProvider.AddonInterface<AddonCharaSelectListMenu>("_CharaSelectListMenu");
        this.addon.AddHandler(AddonEvent.PreFinalize, this.HandleFinalize);
        this.addon.AddHandler(AddonEvent.PostUpdate, this.Handle);
    }

    public void Dispose()
    {
        this.addon.Dispose();
    }

    internal IReadOnlyList<Player> Players()
    {
        var uiModule = UIModule.Instance();
        if (uiModule == null)
        {
            return [];
        }

        var agentModule = uiModule->GetAgentModule();
        if (agentModule == null)
        {
            return [];
        }

        var agentLobby = agentModule->GetAgentLobby();
        if (agentLobby == null)
        {
            return [];
        }

        var entries = agentLobby->LobbyData.CharaSelectEntries;

        List<Player> list = [];
        foreach (var entry in entries)
        {
            var name = entry.Value->NameString;
            var homeWorld = WorldHelper.Worlds.Find(entry.Value->HomeWorldNameString);
            var currentWorld = WorldHelper.Worlds.Find(entry.Value->CurrentWorldNameString);
            if (name != null && name != string.Empty && homeWorld != null && currentWorld != null)
            {
                list.Add(new Player(name, currentWorld, homeWorld));
            }
        }

        return list;
    }

    internal void Subscribe(Action action)
    {
        this.addon.AddHandler(AddonEvent.PostSetup, _ => action.Invoke());
        this.addon.AddHandler(AddonEvent.PreFinalize, _ => action.Invoke());
        this.addon.AddHandler(AddonEvent.PostRefresh, _ => action.Invoke());
        this.addon.AddHandler(AddonEvent.PostRequestedUpdate, _ => action.Invoke());
    }

    internal bool IsVisible()
    {
        return this.addonPtr != null && this.addonPtr->AtkUnitBase.IsVisible && this.addonPtr->CharaList != null;
    }

    internal void OnReady(OnCharacterListReady actions)
    {
        this.onReady.Add(actions);
    }

    internal IReadOnlyList<Vector2> GetPositions()
    {
        try
        {
            if (!this.IsVisible())
            {
                return [];
            }

            List<Vector2> positions = [];
            var length = this.addonPtr->CharaList->ListLength;
            for (var i = 0; i < length; i++)
            {
                positions.Add(new Vector2(
                    this.addonPtr->CharaList->GetItemRenderer(i)->OwnerNode->ScreenX,
                    this.addonPtr->CharaList->GetItemRenderer(i)->OwnerNode->ScreenY));
            }

            Plugin.Log.Info($"CharacterList has {length} entries");
            return positions;
        }
        catch (Exception ex)
        {
            Plugin.Log.Error(ex, "Error getting character list positions");
            return [];
        }
    }

    private void Handle(AddonCharaSelectListMenu* menu)
    {
        this.addonPtr = menu;
        if (this.onReady.Count == 0 || menu->CharaList == null)
        {
            return;
        }

        var length = menu->CharaList->ListLength;
        if (length == 0)
        {
            return;
        }

        var players = this.Players();
        if (players.Count == 0)
        {
            return;
        }

        var data = new CharacterListData(players);
        foreach (var actions in this.onReady)
        {
            actions.Invoke(data);
        }

        this.onReady.Clear();
    }

    private void HandleFinalize(AddonCharaSelectListMenu* addon)
    {
        this.addonPtr = null;
    }
}
