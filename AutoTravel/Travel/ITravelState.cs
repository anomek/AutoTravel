namespace AutoTravel.Travel;

internal interface ITravelState
{
    internal bool Traveling { get; }

    internal string TargetDC { get; }

    internal string? TargetWorld { get; }
}
