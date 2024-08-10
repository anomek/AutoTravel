using System.Collections.Generic;

using AutoTravel.Travel;
using AutoTravel.Travel.WorldHelpers;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;

namespace AutoTravel.GameSystems;

internal class WhereIAm(IClientState clientState, ICondition condition)
{
    private static readonly ConditionFlag[] Conditions = [
        ConditionFlag.BetweenAreas, ConditionFlag.BetweenAreas51, ConditionFlag.BoundByDuty, ConditionFlag.BoundByDuty56, ConditionFlag.BoundByDuty95,
        ConditionFlag.CreatingCharacter, ConditionFlag.DutyRecorderPlayback, ConditionFlag.EditingPortrait, ConditionFlag.InCombat, ConditionFlag.InDeepDungeon, ConditionFlag.InDuelingArea,
        ConditionFlag.InDutyQueue, ConditionFlag.OccupiedInCutSceneEvent, ConditionFlag.OccupiedInEvent, ConditionFlag.OccupiedInQuestEvent, ConditionFlag.ParticipatingInCustomMatch,
        ConditionFlag.PlayingLordOfVerminion, ConditionFlag.PlayingMiniGame, ConditionFlag.ReadyingVisitOtherWorld, ConditionFlag.RegisteringForRaceOrMatch, ConditionFlag.RegisteringForTripleTriadMatch,
        ConditionFlag.WaitingForDuty, ConditionFlag.WaitingForDutyFinder, ConditionFlag.WaitingForRaceOrMatch, ConditionFlag.WaitingForTripleTriadMatch, ConditionFlag.WaitingForTripleTriadMatch83,
        ConditionFlag.WaitingToVisitOtherWorld, ConditionFlag.WatchingCutscene, ConditionFlag.WatchingCutscene78
    ];

    private static readonly HashSet<ConditionFlag> ConditionsSet;

    static WhereIAm()
    {
        ConditionsSet = [];
        foreach (var item in Conditions)
        {
            ConditionsSet.Add(item);
        }
    }

    private readonly IClientState clientState = clientState;
    private readonly ICondition condition = condition;

    internal Player? GetPlayerLocation()
    {
        var localPlayer = this.clientState.LocalPlayer;
        if (localPlayer == null)
        {
            return null;
        }

        var current = WorldHelper.Worlds.Find(localPlayer.CurrentWorld.GameData);
        var home = WorldHelper.Worlds.Find(localPlayer.HomeWorld.GameData);
        return current == null || home == null
            ? null
            : new Player(localPlayer.Name.ToString(), current, home);
    }

    internal bool IsInGameReadyToTravel()
    {
        return this.clientState.IsLoggedIn
            && !this.clientState.IsPvPExcludingDen
            && !this.condition.Any(Conditions);
    }

    internal void Subscribe(System.Action onChange)
    {
        this.condition.ConditionChange += (type, value) =>
        {
            if (ConditionsSet.Contains(type))
            {
                onChange.Invoke();
            }
        };

        this.clientState.Login += onChange;
        this.clientState.Logout += onChange;
        this.clientState.LeavePvP += onChange;
        this.clientState.EnterPvP += onChange;
    }
}
