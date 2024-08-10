using System.Collections.Generic;

using AutoTravel.Utils;

namespace AutoTravel.GameSystems.Steps;

internal class StepList : BaseStep
{
    internal delegate BaseStep CreateStep(IStepActions actions);

    private readonly IReadOnlyList<BaseStep> steps;
    private int step;

    internal StepList(EventLoop eventLoop, IStepActions actions, params CreateStep[] steps)
        : base(eventLoop, actions)
    {
        var innerActions = new StepActions(this.OnSuccess, actions.OnFail);
        var stepsList = new BaseStep[steps.Length];
        for (var i = 0; i < steps.Length; i++)
        {
            stepsList[i] = steps[i](innerActions);
        }

        this.steps = stepsList;
    }

    public override void Dispose()
    {
        foreach (var step in this.steps)
        {
            step.Dispose();
        }
    }

    protected override void RunInternal()
    {
        this.step = 0;
        this.OnSuccess();
    }

    protected override void CancelInternal()
    {
        if (this.step > 0)
        {
            this.steps[this.step - 1].Cancel();
        }

        this.step = -1;
    }

    private void OnSuccess()
    {
        if (this.step >= this.steps.Count)
        {
            this.Success();
        }
        else if (this.step >= 0)
        {
            this.steps[this.step].Run();
            this.step++;
        }
    }
}
