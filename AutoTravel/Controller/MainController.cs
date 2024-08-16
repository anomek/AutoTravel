using System.Collections.Generic;

using AutoTravel.Travel;
using AutoTravel.Windows;

namespace AutoTravel.Controller;

public class MainController
{
    internal UI UI { get; set; } = null!;

    internal TravelList TravelList { get; set; } = null!;

    internal Conductor Conductor { get; set; } = null!;

    internal Notifications Notifications { get; set; } = null!;

    internal IReadOnlyList<DestinationDc> AvailableDestinations(int index)
    {
        return this.TravelList.FindAvailableDestinations(index);
    }

    internal void TravelToDc(int index, DestinationDc dc)
    {
        var player = this.TravelList.GetPlayer(index);
        if (player != null)
        {
            this.Conductor.TravelToWorld(player, dc, null);
        }
    }

    internal void TravelToWorld(int index, DestinationDc dc, string world)
    {
        var player = this.TravelList.GetPlayer(index);
        if (player != null)
        {
            this.Conductor.TravelToWorld(player, dc, world);
        }
        else
        {
            this.Notifications.OnTravelError("Can't start travel: unable to find player");
        }
    }

    internal void Init()
    {
        this.TravelList.Subscribe(this.Refresh);
        this.Conductor.OnTravelStart += this.Refresh;
        this.Conductor.OnTravelEnd += this.Refresh;
        this.Conductor.OnTravelCanceled += this.Refresh;
        this.Conductor.OnTravelFail += _ => this.Refresh();
        this.Notifications.Init(this.Conductor);
        this.Refresh();
    }

    internal void Refresh()
    {
        this.UI.TravelButtonOpen(this.TravelList.IsInWorldReadyToTravel() && Plugin.Configuration.ButtonInGameShow);
        if (this.State.Traveling)
        {
            this.UI.TravelButtonsOpen([]);
            this.UI.ToggleMainUI(true);
        }
        else
        {
            this.UI.TravelButtonsOpen(this.TravelList.GetCharactersListPositions());
            this.UI.ToggleMainUI(false);
        }
    }

    internal void AlignCharacterButtons(float x)
    {
        this.UI.AlignTravelButtons(x);
    }

    internal void CancelTravel()
    {
        this.Conductor.CancelTravel();
    }

    internal ITravelState State => this.Conductor.TravelState;
}
