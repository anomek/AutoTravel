using AutoTravel.Travel.WorldHelpers;

namespace AutoTravel.Travel;

internal record Player(
    string Name,
    World Current,
    World Home)
{
}
