using System;
using System.Collections.Concurrent;
using System.Threading;

namespace AutoTravel.Utils;

internal class EventLoop : IDisposable
{
    private readonly Thread thread;
    private readonly BlockingCollection<Action> queue = [];
    private readonly CancellationTokenSource token = new();

    internal EventLoop()
    {
        this.thread = new Thread(this.Run);
        this.thread.Start();
    }

    public void Dispose()
    {
        this.token.Cancel();
        this.token.Dispose();
        this.queue.Dispose();
    }

    internal void Add(Action action)
    {
        this.queue.Add(action);
    }

    private void Run()
    {
        try
        {
            foreach (var action in this.queue.GetConsumingEnumerable(this.token.Token))
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Plugin.Log.Error(ex, "Error executing action in event loop");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // ignore
        }
    }
}
