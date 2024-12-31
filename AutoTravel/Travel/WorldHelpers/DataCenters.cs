using System.Collections.Generic;

using Lumina.Excel.Sheets;

namespace AutoTravel.Travel.WorldHelpers;

internal class DataCenters
{
    private readonly Dictionary<uint, DataCenter> byId = [];
    private readonly Dictionary<string, DataCenter> byName = [];

    internal DataCenter Register(WorldDCGroupType dc, Region region, IReadOnlyList<World> worlds)
    {
        var name = dc.Name.ToString();
        var dataCenter = new DataCenter(dc.RowId, name, region, worlds);
        this.byName[name] = dataCenter;
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
