using System.Collections.Generic;

namespace AutoTravel.Travel.WorldHelpers;

internal class Worlds
{
    private readonly Dictionary<string, World> byName = [];

    internal World? Find(Lumina.Excel.GeneratedSheets.World? gameData)
    {
        return this.Find(gameData?.Name?.ToString());
    }

    internal World? Find(string? name)
    {
        return name == null ? null : this.byName.GetValueOrDefault(name);
    }

    internal World Register(Lumina.Excel.GeneratedSheets.World world, DataCenter dataCenter)
    {
        var worldObj = new World(world.Name, dataCenter);
        this.byName[world.Name] = worldObj;
        return worldObj;
    }
}
