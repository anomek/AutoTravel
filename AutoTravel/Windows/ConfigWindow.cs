using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoTravel.Controller;
using Dalamud.Interface;
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
        ImGui.Text("AutoTravel button in game");
        ImGui.SameLine();
        HelpIcon("You can move AutoTravel button around by dragging it with right mouse button");
        ImGui.Separator();

        var buttonInGameShow = this.configuration.ButtonInGameShow;
        if (ImGui.Checkbox("Show", ref buttonInGameShow))
        {
            this.configuration.ButtonInGameShow = buttonInGameShow;
            this.configuration.Save();
            this.mainController.Refresh();
        }

        var buttonInGameScaleWithUi = this.configuration.ButtonInGameScaleWithUi;
        if (ImGui.Checkbox("Scale size with game UI settings###ScaleWithUiInGame", ref buttonInGameScaleWithUi))
        {
            this.configuration.ButtonInGameScaleWithUi = buttonInGameScaleWithUi;
            this.configuration.Save();
        }

        var buttonInGameScale = this.configuration.ButtonInGameScale;
        if (ImGui.SliderInt("Scale###ScaleInGmae", ref buttonInGameScale, 50, 300))
        {
            this.configuration.ButtonInGameScale = buttonInGameScale;
            this.configuration.Save();
        }

        ImGui.Separator();
        ImGui.Separator();

        ImGui.NewLine();
        ImGui.Text("AutoTravel button on character list");
        ImGui.SameLine();
        HelpIcon("You can move AutoTravel button around by dragging it with right mouse button");
        ImGui.Separator();

        var buttonInLobbyCopySettings = this.configuration.ButtonInLobbyCopySettings;
        if (ImGui.Checkbox("Same settings as button in game", ref buttonInLobbyCopySettings))
        {
            this.configuration.ButtonInLobbyCopySettings = buttonInLobbyCopySettings;
            this.configuration.Save();
        }

        if (!this.configuration.ButtonInLobbyCopySettings)
        {
            var buttonInLobbyScaleWithUi = this.configuration.ButtonInLobbyScaleWithUi;
            if (ImGui.Checkbox("Scale size with game UI settings###ScaleWithUiInLobby", ref buttonInLobbyScaleWithUi))
            {
                this.configuration.ButtonInLobbyScaleWithUi = buttonInLobbyScaleWithUi;
                this.configuration.Save();
            }

            var buttonInLobbyScale = this.configuration.ButtonInLobbyScale;
            if (ImGui.SliderInt("Scale###ScaleInLobby", ref buttonInLobbyScale, 50, 300))
            {
                this.configuration.ButtonInLobbyScale = buttonInLobbyScale;
                this.configuration.Save();
            }
        }

        ImGui.Separator();
        ImGui.Separator();
        ImGui.NewLine();

        bool retryConfestedDataCenter = this.configuration.RetryCongestedDataCenter;
        if (ImGui.Checkbox("Retry travel to congested data centers", ref retryConfestedDataCenter))
        {
            this.configuration.RetryCongestedDataCenter = retryConfestedDataCenter;
            this.configuration.Save();
        }

        ImGui.SameLine();
        HelpIcon("Plugin will keep trying to travel to congested data center until it succeeds or travel is canceled");
    }

    public void Open()
    {
        this.IsOpen = true;
    }

    private static void HelpIcon(string message)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        ImGui.TextUnformatted(FontAwesomeIcon.QuestionCircle.ToIconString());
        ImGui.PopFont();
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted(message);
            ImGui.EndTooltip();
        }
    }
}
