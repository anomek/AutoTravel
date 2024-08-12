using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AutoTravel.Utils;

namespace AutoTravel.GameSystems.Steps;

internal class DelayStep : BaseStep
{
    private readonly CancellationTokenSource token = new();
    private readonly TimeSpan timeSpan;

    private bool running;
    

    public DelayStep(TimeSpan timeSpan, EventLoop eventLoop, IStepActions actions)
        : base(eventLoop, actions)
    {
        this.timeSpan = timeSpan;
    }

    public override void Dispose()
    {
    }

    protected override void CancelInternal()
    {
        this.running = false;
        this.token.Cancel();
    }

    protected override void RunInternal()
    {
        this.running = true;
        Plugin.Framework.RunOnTick(this.Handle, this.timeSpan, 0, this.token.Token);
    }

    private void Handle()
    {
        if (this.running)
        {
            this.running = false;
            this.Success();
        }
    }
}
