using System;
using System.Collections.Generic;
using System.Numerics;

using AutoTravel.Controller;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;

namespace AutoTravel.Windows;

internal class UI : IDisposable
{
    private readonly WindowSystem windowSystem = new("AutoTravel");
    private readonly MainController controller;
    private readonly Tools tools;

    private MainWindow MainWindow { get; init; }

    private TravelButtonWindow TravelButton { get; init; }

    private TravelButtonWindow[] TravelButtons { get; init; }

    internal UI(MainController controller, ITextureProvider textureProvider)
    {
        this.controller = controller;
        this.tools = new Tools(textureProvider);

        this.MainWindow = new MainWindow(controller);
        this.TravelButton = new TravelButtonWindow(-1, this.tools, this.controller);
        this.TravelButtons = new TravelButtonWindow[8];
        for (var i = 0; i < this.TravelButtons.Length; i++)
        {
            this.TravelButtons[i] = new TravelButtonWindow(i, this.tools, this.controller);
            this.windowSystem.AddWindow(this.TravelButtons[i]);
        }

        this.windowSystem.AddWindow(this.MainWindow);
        this.windowSystem.AddWindow(this.TravelButton);
    }

    public void Dispose()
    {
        this.windowSystem.RemoveAllWindows();
        this.MainWindow.Dispose();
        this.TravelButton.Dispose();
        for (var i = 0; i < this.TravelButtons.Length; i++)
        {
            this.TravelButtons[i].Dispose();
        }
    }

    internal void Register(IUiBuilder uiBuilder)
    {
        uiBuilder.Draw += this.windowSystem.Draw;
    }

    internal void ToggleMainUI(bool open)
    {
        this.MainWindow.IsOpen = open;
    }

    internal void TravelButtonOpen(bool open)
    {
        this.TravelButton.IsOpen = open;
    }

    internal void TravelButtonsOpen(IReadOnlyList<Vector2> positons)
    {
        for (var i = 0; i < this.TravelButtons.Length; i++)
        {
            if (i < positons.Count)
            {
                this.TravelButtons[i].IsOpen = true;
                this.TravelButtons[i].SetPositionY(positons[i].Y);
            }
            else
            {
                this.TravelButtons[i].IsOpen = false;
            }
        }
    }

    internal void AlignTravelButtons(float x)
    {
        for (var i = 0; i < this.TravelButtons.Length; i++)
        {
            this.TravelButtons[i].SetPositionX(x);
        }
    }
}
