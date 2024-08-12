using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTravel.GameSystems;

internal record StepFailure(string Message)
{
    internal static readonly StepFailure WorldTravelCooldown = new("Can't travel right now: wait 1 minute and try again");
    internal static readonly StepFailure DataCenterWorldsUnavailable = new("All worlds on seleced Data Center are full");
    internal static readonly StepFailure LogoutCanceled = new("Logout canceled");
    internal static readonly StepFailure DKTExecGenericFailure = new("Can't travel to selected Data Center");
    internal static readonly StepFailure DataCenterUnavailable = new("Selected Data Center is unavilable as a travel destination");
    internal static readonly StepFailure LogoutUnavailable = new("Can't log out from game. Log out manually and travel from character selection menu");
    internal static readonly StepFailure AlreadyTraveling = new("Can't start auto travel: already active");
    internal static readonly StepFailure DataCenterNotSelected = new("Target Data Center was not selected");
    internal static readonly StepFailure CharacterNotFound = new("Can't find traveling character");

    public override string ToString()
    {
        return this.Message;
    }
}
