using System.Collections.Generic;

namespace AutoTravel.Travel;

internal record CharacterListData(IReadOnlyList<Player> Characters)
{
    public override string ToString()
    {
        var s = string.Empty;
        foreach (var p in this.Characters)
        {
            s += p.ToString() + " ";
        }

        return s;
    }
}
