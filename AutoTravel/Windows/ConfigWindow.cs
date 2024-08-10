using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoTravel.Controller;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace AutoTravel.Windows;

internal class ConfigWindow : Window
{
    private readonly Configuration configuration = Plugin.Configuration;
    private readonly MainController mainController;

    internal ConfigWindow(MainController mainController)
        : base("AutoTravel Configuration", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.AlwaysAutoResize)
    {
        this.mainController = mainController;
    }

    public override void Draw()
    {
        bool showAutoTravelButtonInGmae = this.configuration.ShowAutoTravelButtonInGame;
        if (ImGui.Checkbox("Show AutoTravel button in game", ref showAutoTravelButtonInGmae))
        {
            this.configuration.ShowAutoTravelButtonInGame = showAutoTravelButtonInGmae;
            this.configuration.Save();
            this.mainController.Refresh();
        }

        ImGui.TextUnformatted("TIP: you can move it around with right mouse button");
    }

    public void Open()
    {
        this.IsOpen = true;
    }
}
