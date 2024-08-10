namespace AutoTravel.Travel.WorldHelpers;

internal class World(string name, DataCenter dataCenter)
{
    internal string Name { get; } = name;

    internal DataCenter DataCenter { get; } = dataCenter;

    public override string ToString()
    {
        return this.Name;
    }
}
