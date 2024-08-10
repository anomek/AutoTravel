using System.Collections.Generic;

namespace AutoTravel.Travel.WorldHelpers
{
    internal class Regions
    {
        private readonly Dictionary<byte, Region> regions = [];

        internal Region? Find(byte id)
        {
            return this.regions.GetValueOrDefault(id);
        }

        internal void Register(byte regionId, List<DataCenter> dataCenters)
        {
            this.regions[regionId] = new Region(regionId, dataCenters);
        }
    }
}
