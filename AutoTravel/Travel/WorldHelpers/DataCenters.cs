using System.Collections.Generic;

namespace AutoTravel.Travel.WorldHelpers;

internal class DataCenters
{
    private readonly Dictionary<uint, DataCenter> byId = [];
    private readonly Dictionary<string, DataCenter> byName = [];

    internal DataCenter Register(Lumina.Excel.GeneratedSheets.WorldDCGroupType dc, Region region, IReadOnlyList<World> worlds)
    {
        var dataCenter = new DataCenter(dc.RowId, dc.Name, region, worlds);
        this.byName[dc.Name] = dataCenter;
        this.byId[dc.RowId] = dataCenter;
        return dataCenter;
    }

    internal DataCenter? Find(string name)
    {
        return this.byName.GetValueOrDefault(name);
    }

    internal DataCenter? Find(uint id)
    {
        return this.byId.GetValueOrDefault(id);
    }
}
