using System.Collections.Generic;

namespace AutoTravel.Travel.WorldHelpers;

internal class DataCenter(uint id, string name, Region region, IReadOnlyList<World> worlds)
{
    internal uint Id { get; } = id;

    internal string Name { get; } = name;

    internal Region Region { get; } = region;

    internal IReadOnlyList<World> Worlds { get; } = worlds;

    public override string ToString()
    {
        return this.Name;
    }
}
