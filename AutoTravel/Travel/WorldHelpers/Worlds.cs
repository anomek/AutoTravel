using System.Collections.Generic;

namespace AutoTravel.Travel.WorldHelpers;

internal class Worlds
{
    private readonly Dictionary<string, World> byName = [];

    internal World? Find(Lumina.Excel.Sheets.World? gameData)
    {
        return this.Find(gameData?.Name.ToString());
    }

    internal World? Find(string? name)
    {
        return name == null ? null : this.byName.GetValueOrDefault(name);
    }

    internal World Register(Lumina.Excel.Sheets.World world, DataCenter dataCenter)
    {
        var name = world.Name.ToString();
        var worldObj = new World(name, dataCenter);
        this.byName[name] = worldObj;
        return worldObj;
    }
}
