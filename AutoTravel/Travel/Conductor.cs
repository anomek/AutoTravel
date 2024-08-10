using System;

using AutoTravel.GameSystems;
using AutoTravel.GameSystems.Steps;
using AutoTravel.Utils;

namespace AutoTravel.Travel;

internal class Conductor : IDisposable
{
    internal event Action? OnTravelStart;

    internal event Action? OnTravelEnd;

    internal event Action<string>? OnTravelFail;

    internal event Action? OnTravelCanceled;

    internal ITravelState TravelState => this.travelState;

    private readonly CharacterList characterList;
    private readonly EventLoop eventLoop;
    private readonly TravelState travelState = new();

    /// <summary>
    /// Log out from game and get into character selection screen.
    /// </summary>
    private readonly BaseStep logoutStep;

    /// <summary>
    /// Click on the character on character selection screen.
    /// </summary>
    private readonly ClickCharacterStep clickCharacterStep;

    /// <summary>
    /// Click on the character context menu in character selection screen.
    /// </summary>
    private readonly ContextMenuStep contextMenuStep;

    /// <summary>
    /// Confirm dialogs required to return to home world within data center.
    /// </summary>
    private readonly BaseStep confirmReturnHome;

    /// <summary>
    /// First confirom dialog.
    /// </summary>
    private readonly BaseStep confirmDcTravel;

    /// <summary>
    /// Select target dc and world or confirm returning to home world.
    /// </summary>
    private readonly SelectWorldStep selectWorldStep;

    /// <summary>
    /// All confirmations after selecting target world until entering character selection screen.
    /// </summary>
    private readonly BaseStep travelStep;

    /// <summary>
    /// Confirm log in into the game.
    /// </summary>
    private readonly BaseStep loginStep;

    internal Conductor(TravelSteps travelSteps, CharacterList characterList)
    {
        this.characterList = characterList;
        this.eventLoop = travelSteps.EventLoop;
        this.logoutStep = new StepList(this.eventLoop, this.StepActions(this.OnCharacterSelectionScreen), travelSteps.SystemMenuStep, travelSteps.LogoutStep, travelSteps.ConfirmYesNo, travelSteps.StartStep);
        this.clickCharacterStep = travelSteps.ClickCharacterStep(this.StepActions(this.OnCharacterClicked));
        this.contextMenuStep = travelSteps.ContextMenuStep(this.StepActions(this.OnContextMenu));
        this.confirmDcTravel = travelSteps.ConfirmDcTravel(this.StepActions(this.OnDcTravelConfirmed));
        this.selectWorldStep = travelSteps.SelectWorldStep(this.StepActions(this.OnSelectWorldStep));
        this.travelStep = new StepList(this.eventLoop, this.StepActions(this.OnCharacterSelectionScreen), travelSteps.ConfirmDcTravelExec, travelSteps.SelectOkStep);
        this.confirmReturnHome = new StepList(this.eventLoop, this.StepActions(this.OnCharacterSelectionScreen), travelSteps.ConfirmHomeTravel, travelSteps.SelectOkStep);
        this.loginStep = travelSteps.ConfirmYesNo(this.StepActions(this.OnLoggedIn));
    }

    public void Dispose()
    {
        this.logoutStep.Dispose();
        this.clickCharacterStep.Dispose();
        this.contextMenuStep.Dispose();
        this.confirmReturnHome.Dispose();
        this.confirmDcTravel.Dispose();
        this.selectWorldStep.Dispose();
        this.travelStep.Dispose();
        this.loginStep.Dispose();
    }

    internal void TravelToWorld(Player from, DestinationDc dc, string? name)
    {
        this.eventLoop.Add(() =>
        {
            if (this.travelState.Traveling)
            {
                this.OnFailure("Can't start auto travel: already active");
                return;
            }

            this.travelState.TargetWorld = name;
            this.travelState.TargetDc = dc;
            this.travelState.PlayerLocation = from;
            this.travelState.Traveling = true;

            this.OnTravelStart?.Invoke();

            if (this.characterList.IsVisible())
            {
                Plugin.Log.Info("Character list is visible. Starting travel by selecting character");
                this.OnCharacterSelectionScreen();
            }
            else
            {
                Plugin.Log.Info("Character list is not visible. Starting travel by loging out");
                this.Run(this.logoutStep);
            }
        });
    }

    internal void CancelTravel()
    {
        this.eventLoop.Add(() =>
        {
            Plugin.Log.Info("canceling travel");
            this.travelState.CurrentStep?.Cancel();
            this.travelState.Traveling = false;
            this.OnTravelCanceled?.Invoke();
        });
    }

    private void OnFailure(string message)
    {
        this.travelState.Traveling = false;
        this.OnTravelFail?.Invoke(message);
    }

    //--------------
    // Callbacks
    //--------------
    private void OnLoggedIn()
    {
        Plugin.Log.Info("Travel finished");
        this.travelState.Traveling = false;
        this.OnTravelEnd?.Invoke();
    }

    private void OnCharacterSelectionScreen()
    {
        if (this.travelState.Traveling)
        {
            this.characterList.OnReady(data => this.eventLoop.Add(() => this.OnCharacterSelectionScreenLoaded(data)));
        }
    }

    private void OnCharacterSelectionScreenLoaded(CharacterListData data)
    {
        if (this.travelState.Traveling)
        {
            var select = -1;
            for (var i = 0; i < data.Characters.Count; i++)
            {
                if (this.travelState.IsTravelingCharacter(data.Characters[i]))
                {
                    select = i;
                    break;
                }
            }

            if (select < 0)
            {
                Plugin.Log.Info("No character maching");
                Plugin.Log.Info(data.ToString());
                this.OnFailure($"Can't find character {this.travelState.PlayerFullName}");
            }
            else
            {
                this.travelState.LatestPlayerLocation = data.Characters[select];
                var rightClick = !this.travelState.IsAtDestination();
                this.clickCharacterStep.SetClick(select, rightClick);
                this.Run(this.clickCharacterStep);
            }
        }
    }

    private void OnCharacterClicked()
    {
        if (this.travelState.Traveling)
        {
            if (this.travelState.IsAtDestination())
            {
                this.Run(this.loginStep);
            }
            else if (this.travelState.IsAtHomeWorld())
            {
                this.contextMenuStep.SetItem(8);
                this.Run(this.contextMenuStep);
            }
            else
            {
                this.contextMenuStep.SetItem(6);
                this.Run(this.contextMenuStep);
            }
        }
    }

    private void OnContextMenu()
    {
        if (this.travelState.Traveling)
        {
            if (this.travelState.IsAtHomeWorld())
            {
                this.Run(this.confirmDcTravel);
            }
            else if (this.travelState.IsAtHomeDc())
            {
                this.Run(this.confirmReturnHome);
            }
            else
            {
                this.selectWorldStep.SelectReturnHome();
                this.Run(this.selectWorldStep);
            }
        }
    }

    private void OnDcTravelConfirmed()
    {
        if (this.travelState.Traveling)
        {
            if (this.travelState.TargetDc == null)
            {
                this.OnFailure("Target Data Center was not selected");
            }
            else
            {
                this.selectWorldStep.Select(this.travelState.TargetDc.Name, this.travelState.TargetDc.Id, this.travelState.TargetWorld);
                this.Run(this.selectWorldStep);
            }
        }
    }

    private void OnSelectWorldStep()
    {
        if (this.travelState.Traveling)
        {
            this.Run(this.travelStep);
        }
    }

    //-------------
    // Other helpers
    //-------------
    private void Run(BaseStep step)
    {
        this.travelState.CurrentStep = step;
        step.Run();
    }

    private StepActions StepActions(Action onSuccess)
    {
        return new StepActions(onSuccess, this.OnFailure);
    }
}
