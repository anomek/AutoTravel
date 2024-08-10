using System;

namespace AutoTravel.GameSystems.Steps;

internal class StepActions(Action success, Action<string> fail) : IStepActions
{
    public void OnFail(string message)
    {
        fail(message);
    }

    public void OnSuccess()
    {
        success();
    }
}
