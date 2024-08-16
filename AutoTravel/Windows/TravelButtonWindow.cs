using System;
using System.Numerics;

using AutoTravel;
using AutoTravel.Controller;
using AutoTravel.Windows;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;

internal class TravelButtonWindow : Window, IDisposable
{
    private readonly Tools tools;
    private readonly MainController controller;

    private readonly bool moveable;
    private readonly int index;

    private float? changePositionX;
    private float? changePositionY;
    private bool firstDraw = true;
    private bool movingShortcut;
    private Vector2 movingMouse;
    private float shortcutOpacity = 0.5f;

    public TravelButtonWindow(int index, Tools tools, MainController controller)
        : base("AutoTravelButton" + index, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoMove, false)
    {
        this.tools = tools;
        this.controller = controller;

        this.moveable = index == -1;
        this.index = index;
    }

    public void Dispose()
    {
    }

    public override void Draw()
    {
        if (this.changePositionX != null || this.changePositionY != null)
        {
            var curPos = ImGui.GetWindowPos();
            ImGui.SetWindowPos(new Vector2(this.changePositionX ?? curPos.X, this.changePositionY ?? curPos.Y));
            this.changePositionX = null;
            this.changePositionY = null;
        }

        if (this.firstDraw)
        {
            if (this.index == 0)
            {
                Plugin.Log.Info($"{ImGui.GetWindowPos().X}");
                this.controller.AlignCharacterButtons(ImGui.GetWindowPos().X);
            }

            this.firstDraw = false;
        }

        this.MainButton();
        this.DrawPopup($"TravelButtonPopup{this.index}");
    }

    internal void SetPositionY(float y)
    {
        this.changePositionY = y - 2;
    }

    internal void SetPositionX(float x)
    {
        this.changePositionX = x;
    }

    private void MainButton()
    {
        var icon = this.tools.GetIconWrap(59285);
        var size = ScaleIcon(icon.Size);
        ImGui.SetWindowSize(size + new Vector2(10, 10));
        ImGui.SetCursorPos(new Vector2(5, 5));
        ImGui.Image(icon.ImGuiHandle, size, new Vector2(0.0f, 0.0f), new Vector2(1.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, this.shortcutOpacity));

        if (ImGui.IsItemHovered() == true)
        {
            if (ImGui.IsItemClicked(ImGuiMouseButton.Right) == true)
            {
                this.movingShortcut = true;
                this.movingMouse = ImGui.GetMousePos();
            }

            if (this.movingShortcut == false)
            {
                ImGui.BeginTooltip();
                ImGui.Text("Data Center Travel");
                ImGui.EndTooltip();
            }

            this.shortcutOpacity += (1.0f - this.shortcutOpacity) / 10.0f;
        }
        else
        {
            this.shortcutOpacity -= (this.shortcutOpacity - 0.5f) / 10.0f;
        }

        if (this.movingShortcut == true)
        {
            if (ImGui.IsMouseDown(ImGuiMouseButton.Right) == true)
            {
                var curmouse = ImGui.GetMousePos();
                var delta = new Vector2(curmouse.X - this.movingMouse.X, this.moveable ? curmouse.Y - this.movingMouse.Y : 0);
                this.movingMouse = curmouse;
                var cpos = ImGui.GetWindowPos();
                var newpos = cpos + delta;
                ImGui.SetWindowPos(newpos);
                if (!this.moveable)
                {
                    this.controller.AlignCharacterButtons(newpos.X);
                }
            }
            else
            {
                this.movingShortcut = false;
            }
        }
    }

    private Vector2 ScaleIcon(Vector2 iconSize)
    {
        var uiScale = 1f;
        if (Plugin.GameConfig.TryGet(Dalamud.Game.Config.SystemConfigOption.UiHighScale,  out uint uiScaleIndex))
        {
            uiScale = uiScaleIndex switch
            {
                0 => 1,
                1 => 1.5f,
                2 => 2,
                3 => 3,
                _ => 1,
            };
        }

        return iconSize * Plugin.Configuration.ScaleButton(this.index, uiScale) / 2;
    }

    private void DrawPopup(string name)
    {
        if (ImGui.BeginPopupContextItem(name, ImGuiPopupFlags.MouseButtonLeft))
        {
            ImGui.Text("Data Center Travel");
            var destinations = this.controller.AvailableDestinations(this.index);
            foreach (var dest in destinations)
            {
                if (dest.IsHome)
                {
                    ImGui.PushFont(UiBuilder.IconFont);
                    ImGui.Text(FontAwesomeIcon.Home.ToIconString());
                    ImGui.PopFont();
                    ImGui.SameLine();
                    if (ImGui.Selectable(dest.Name))
                    {
                        this.controller.TravelToDc(this.index, dest);
                        ImGui.CloseCurrentPopup();
                    }
                }
                else
                {
                    var header = ImGui.CollapsingHeader($"##{dest.Name}{this.index}", ImGuiTreeNodeFlags.OpenOnArrow);
                    ImGui.SameLine();
                    ImGui.Selectable(dest.Name);
                    if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenBlockedByActiveItem) && ImGui.IsMouseClicked(ImGuiMouseButton.Left, false))
                    {
                        Plugin.Log.Info($"travel to dc {dest.Name}");
                        this.controller.TravelToDc(this.index, dest);
                        ImGui.CloseCurrentPopup();
                    }

                    if (header)
                    {
                        foreach (var world in dest.DataCenter.Worlds)
                        {
                            if (ImGui.Selectable(world.Name))
                            {
                                this.controller.TravelToWorld(this.index, dest, world.Name);
                            }
                        }
                    }
                }
            }

            ImGui.EndPopup();
        }
    }
}
