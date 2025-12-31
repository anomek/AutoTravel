using System.Collections.Generic;

using AutoTravel.Travel;
using AutoTravel.Travel.WorldHelpers;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;

namespace AutoTravel.GameSystems;

internal class WhereIAm(IPlayerState playerState, ICondition condition)
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

    private readonly IPlayerState playerState = playerState;
    private readonly ICondition condition = condition;

    internal Player? GetPlayerLocation()
    {
        if (!this.playerState.IsLoaded)
        {
            return null;
        }

        var current = WorldHelper.Worlds.Find(this.playerState.CurrentWorld.ValueNullable);
        var home = WorldHelper.Worlds.Find(this.playerState.HomeWorld.ValueNullable);
        return current == null || home == null
            ? null
            : new Player(this.playerState.CharacterName, current, home);
    }

    internal bool IsInGameReadyToTravel()
    {
        return this.playerState.IsLoaded
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

        // IClientState got deprecated and i can't find good replacements for these events
        // this.playerState.Login += onChange;
        // this.playerState.Logout += (type, code) => { onChange(); };
        // this.playerState.LeavePvP += onChange;
        // this.playerState.EnterPvP += onChange;
    }
}
