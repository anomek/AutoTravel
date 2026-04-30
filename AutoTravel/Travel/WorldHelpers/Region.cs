using System.Collections.Generic;

namespace AutoTravel.Travel.WorldHelpers;

internal class Region(uint id, IReadOnlyList<DataCenter> dataCenters)
{
    internal uint Id { get; } = id;

    internal IReadOnlyList<DataCenter> DataCenters { get; } = dataCenters;

    public override string ToString()
    {
        return this.Id.ToString();
    }
}
