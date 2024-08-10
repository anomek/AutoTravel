using AutoTravel.GameSystems.Steps;

namespace AutoTravel.Travel;

internal class TravelState : ITravelState
{
    public bool Traveling { get; set; }

    public DestinationDc? TargetDc { get; set; }

    public string? TargetWorld { get; set; }

    public Player? PlayerLocation { get; set; }

    public Player? LatestPlayerLocation { get; set; }

    public BaseStep? CurrentStep { get; set; }

    public string TargetDC => this.TargetDc?.Name ?? "unknwon";

    public string PlayerFullName => this.PlayerLocation == null ? "unknown" : $"{this.PlayerLocation.Name}@{this.PlayerLocation.Home.Name}";

    public bool IsAtDestination()
    {
        return this.LatestPlayerLocation != null
            && this.TargetDc != null
            && this.LatestPlayerLocation.Current.DataCenter == this.TargetDc.DataCenter;
    }

    public bool IsTravelingCharacter(Player player)
    {
        return this.PlayerLocation != null
            && player.Name == this.PlayerLocation.Name
            && player.Home == this.PlayerLocation.Home;
    }

    public bool IsAtHomeDc()
    {
        return this.LatestPlayerLocation != null
            && this.LatestPlayerLocation.Current.DataCenter == this.LatestPlayerLocation.Home.DataCenter;
    }

    public bool IsAtHomeWorld()
    {
        return this.LatestPlayerLocation != null
            && this.LatestPlayerLocation.Current.Name == this.LatestPlayerLocation.Home.Name;
    }
}
