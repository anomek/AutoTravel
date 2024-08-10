using System.Collections.Generic;

namespace AutoTravel.Travel.WorldHelpers;

internal class Region(byte id, IReadOnlyList<DataCenter> dataCenters)
{
    internal byte Id { get; } = id;

    internal IReadOnlyList<DataCenter> DataCenters { get; } = dataCenters;

    public override string ToString()
    {
        return this.Id.ToString();
    }
}
