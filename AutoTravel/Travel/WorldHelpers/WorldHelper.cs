using System.Collections.Generic;

using Lumina.Excel.GeneratedSheets;

namespace AutoTravel.Travel.WorldHelpers;

internal class WorldHelper
{
    internal static Regions Regions { get; } = new();

    internal static DataCenters DataCenters { get; } = new();

    internal static Worlds Worlds { get; } = new();

    internal static void Init()
    {
        Dictionary<byte, List<DataCenter>> dcPerRegion = [];
        Dictionary<uint, List<World>> worldPerDc = [];
        foreach (var regionId in new List<byte> { 1, 2, 3, 4 })
        {
            dcPerRegion[regionId] = [];
            Regions.Register(regionId, dcPerRegion[regionId]);
        }

        foreach (var dc in (IEnumerable<WorldDCGroupType>?)Plugin.DataManager.GetExcelSheet<WorldDCGroupType>() ?? [])
        {
            var region = Regions.Find(dc.Region);
            if (region != null)
            {
                worldPerDc[dc.RowId] = [];

                var dataCenter = DataCenters.Register(dc, region, worldPerDc[dc.RowId]);
                dcPerRegion[dc.Region].Add(dataCenter);
            }
        }

        foreach (var world in (IEnumerable<Lumina.Excel.GeneratedSheets.World>?)Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.World>() ?? [])
        {
            var dcId = world.DataCenter.Row;
            var dataCenter = DataCenters.Find(dcId);
            if (dataCenter != null && world.IsPublic)
            {
                var worldObj = Worlds.Register(world, dataCenter);
                worldPerDc[dcId].Add(worldObj);
            }
        }
    }
}
