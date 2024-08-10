using System;

using AutoTravel.Utils;

namespace AutoTravel.GameSystems.Steps;

internal abstract class BaseStep(EventLoop eventLoop, IStepActions actions)
    : IDisposable
{
    private readonly EventLoop eventLoop = eventLoop;
    private readonly IStepActions actions = actions;

    public abstract void Dispose();

    internal void Run()
    {
        this.eventLoop.Add(this.RunInternal);
    }

    internal void Cancel()
    {
        this.eventLoop.Add(this.CancelInternal);
    }

    protected void Success()
    {
        this.eventLoop.Add(this.actions.OnSuccess);
    }

    protected void Fail(string message)
    {
        this.eventLoop.Add(() => this.actions.OnFail(message));
    }

    protected void AddToEventLoop(Action action)
    {
        this.eventLoop.Add(action);
    }

    protected abstract void RunInternal();

    protected abstract void CancelInternal();
}
