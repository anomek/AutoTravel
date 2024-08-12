using System;

namespace AutoTravel.GameSystems.Steps;

internal class StepActions(Action success, Action<StepFailure> fail) : IStepActions
{
    public void OnFail(StepFailure message)
    {
        fail(message);
    }

    public void OnSuccess()
    {
        success();
    }
}
