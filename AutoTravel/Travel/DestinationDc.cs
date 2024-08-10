using AutoTravel.Travel.WorldHelpers;

namespace AutoTravel.Travel;

internal record DestinationDc(
    DataCenter DataCenter,
    bool IsHome)
{
    internal string Name => this.DataCenter.Name;

    internal uint Id => this.DataCenter.Id;
}
