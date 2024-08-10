using System;
using System.Numerics;

using AutoTravel.Controller;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace AutoTravel.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly MainController controller;

    public MainWindow(MainController controller)
        : base("Traveling##Main window", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.AlwaysAutoResize)
    {
        this.controller = controller;
        this.ShowCloseButton = false;
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(10, 10),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue),
        };
    }

    public void Dispose()
    {
    }

    public override void PreDraw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowTitleAlign, new Vector2(0.5f, 0.5f));
        base.PreDraw();
    }

    public override void PostDraw()
    {
        ImGui.PopStyleVar();
        base.PostDraw();
    }

    public override void Draw()
    {
        if (this.controller.State.Traveling)
        {
            if (ImGui.BeginTable("travelingTable", 2))
            {
                ImGui.TableNextColumn();
                ImGui.TextUnformatted("Target DC: ");
                ImGui.TableNextColumn();
                ImGui.TextUnformatted(this.controller.State.TargetDC);
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.TextUnformatted("Target World: ");
                ImGui.TableNextColumn();
                ImGui.TextUnformatted(this.controller.State.TargetWorld ?? "first available");
                ImGui.EndTable();
            }

            ImGui.PushStyleVar(ImGuiStyleVar.ButtonTextAlign, new Vector2(.5f, .5f));
            var available = ImGui.GetContentRegionAvail();
            if (ImGui.Button("Cancel", new Vector2(available.X, 0)))
            {
                this.controller.CancelTravel();
            }

            ImGui.PopStyleVar();
        }
    }
}
