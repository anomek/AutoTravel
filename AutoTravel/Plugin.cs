using AutoTravel.Controller;
using AutoTravel.GameSystems;
using AutoTravel.GameSystems.Addons;
using AutoTravel.Travel;
using AutoTravel.Travel.WorldHelpers;
using AutoTravel.Utils;
using AutoTravel.Windows;
using Dalamud.Game;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

#pragma warning disable SA1134 // Attributes should not share line

namespace AutoTravel;

public sealed unsafe class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;

    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;

    [PluginService] internal static IAddonLifecycle AddonLifecycle { get; private set; } = null!;

    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;

    [PluginService] internal static IPluginLog Log { get; private set; } = null!;

    [PluginService] internal static IAddonEventManager AddonEventManager { get; private set; } = null!;

    [PluginService] internal static IGameInteropProvider GameInterop { get; private set; } = null!;

    [PluginService] internal static ISigScanner SigScanner { get; private set; } = null!;

    [PluginService] internal static IClientState ClientState { get; private set; } = null!;

    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;

    [PluginService] internal static IToastGui Toast { get; private set; } = null!;

    [PluginService] internal static IGameGui GameGui { get; private set; } = null!;

    [PluginService] internal static ICondition Condition { get; private set; } = null!;

    [PluginService] internal static INotificationManager NotificationManager { get; private set; } = null!;

    [PluginService] internal static IFramework Framework { get; private set; } = null!;

#pragma warning restore SA1134 // Attributes should not share line

    internal static Configuration Configuration { get; private set; } = null!;

    private readonly MainController mainController = new();
    private readonly UI ui;

    private readonly AddonInterface<AddonLobbyWKTCheckHome> debugAddon;

    private readonly EventLoop eventLoop = new();
    private readonly Conductor conductor;

    public Plugin()
    {
        WorldHelper.Init();

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        this.ui = new(this.mainController, TextureProvider);

        var addonInterfaceProvider = new AddonInterfaceProvider(AddonLifecycle, AddonEventManager);
        var whereIAm = new WhereIAm(ClientState, Condition);
        var travelActions = new TravelSteps(GameGui, addonInterfaceProvider, this.eventLoop);
        var characterList = new CharacterList(addonInterfaceProvider);

        var travelList = new TravelList(whereIAm, characterList);
        this.conductor = new Conductor(travelActions, characterList);

        this.mainController.UI = this.ui;
        this.mainController.TravelList = travelList;
        this.mainController.Conductor = this.conductor;
        this.mainController.Notifications = new Notifications(NotificationManager);
        this.mainController.Init();
        this.ui.Register(PluginInterface.UiBuilder);

        this.debugAddon = addonInterfaceProvider.AddonInterface<AddonLobbyWKTCheckHome>("LobbyWKTCheckHome");

        // this.debugAddon.AddHandler(AddonEvent.PostSetup, this.DebugAddon);
#if DEBUG
        PluginInterface.OpenDeveloperMenu();
        Callbacks.InstallHook();
#endif
    }

    public void Dispose()
    {
        this.ui.Dispose();
        this.eventLoop.Dispose();
        this.conductor.Dispose();
        Callbacks.Dispose();

        this.debugAddon.Dispose();
    }

    private void DebugAddon(AddonLobbyWKTCheckHome* addon)
    {
        var basePointer = (nint*)addon;
        for (var i = 0; i < 100; i++)
        {
            Log.Info($"{i * 8:x}: {basePointer[i]:x}");
        }
    }
}
