using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using AutoTravel.GameSystems;
using AutoTravel.Travel.WorldHelpers;

namespace AutoTravel.Travel;

internal class TravelList(WhereIAm whereIAm, CharacterList characterList)
{
    private readonly WhereIAm whereIAm = whereIAm;
    private readonly CharacterList characterList = characterList;

    internal IReadOnlyList<DestinationDc> FindAvailableDestinations(int index)
    {
        var player = this.GetPlayer(index);
        if (player == null)
        {
            return [];
        }

        var destionationDcs = new HashSet<DataCenter>();
        foreach (var dc in player.Home.DataCenter.Region.DataCenters)
        {
            destionationDcs.Add(dc);
        }

        var materiaDc = WorldHelper.DataCenters.Find("Materia");
        if (materiaDc != null)
        {
            destionationDcs.Add(materiaDc);
        }

        destionationDcs.Remove(player.Current.DataCenter);

        return destionationDcs
            .Select(dc => new DestinationDc(dc, dc == player.Home.DataCenter))
            .OrderBy(dc => (dc.DataCenter.Region.Id << 8) & dc.Id)
            .ToList();
    }

    internal IReadOnlyList<Vector2> GetCharactersListPositions()
    {
        return this.characterList.GetPositions();
    }

    internal Player? GetPlayer(int index)
    {
        if (index == -1)
        {
            return this.whereIAm.GetPlayerLocation();
        }
        else
        {
            var players = this.characterList.Players();
            if (index < players.Count)
            {
                return players[index];
            }
        }

        return null;
    }

    internal bool IsInWorldReadyToTravel()
    {
        return this.whereIAm.IsInGameReadyToTravel();
    }

    internal void Subscribe(Action refresh)
    {
        this.whereIAm.Subscribe(refresh);
        this.characterList.Subscribe(refresh);
    }
}
