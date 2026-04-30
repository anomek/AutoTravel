using System.Collections.Generic;

namespace AutoTravel.Travel.WorldHelpers
{
    internal class Regions
    {
        private readonly Dictionary<uint, Region> regions = [];

        internal Region? Find(uint id)
        {
            return this.regions.GetValueOrDefault(id);
        }

        internal void Register(uint regionId, List<DataCenter> dataCenters)
        {
            this.regions[regionId] = new Region(regionId, dataCenters);
        }
    }
}
