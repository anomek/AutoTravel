using System;
using System.Collections.Generic;

using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
namespace AutoTravel.GameSystems;

public unsafe class AddonInterface<TAddon> : IDisposable
{
    private static readonly HashSet<AddonEvent> AllowedEvents = [AddonEvent.PostSetup, AddonEvent.PreFinalize, AddonEvent.PostUpdate, AddonEvent.PostRefresh, AddonEvent.PostRequestedUpdate];

    private readonly IAddonLifecycle addonLifecycle;
    private readonly IAddonEventManager eventManager;

    private readonly IAddonLifecycle.AddonEventDelegate internalAddonEventDelegate;
    private readonly Dictionary<AddonEvent, List<Callback>> registeredCallbacks = [];

    private readonly List<IAddonEventHandle> internalRegisteredEventHandlers = [];
    private readonly List<EventRegistration<TAddon>> eventRegistrations = [];

    public delegate void Callback(TAddon* addon);

    public delegate nint ComponentSelector(TAddon* addon);

    private record EventRegistration<T>(AddonInterface<T>.ComponentSelector Component, AddonEventType EventType, AddonInterface<T>.Callback Callback)
    {
        internal nint GetComponent(T* addon)
        {
            return this.Component.Invoke(addon);
        }
    }

    public AddonInterface(IAddonLifecycle addonLifecycle, IAddonEventManager eventManager, string name)
    {
        this.addonLifecycle = addonLifecycle;
        this.eventManager = eventManager;
        this.internalAddonEventDelegate = new IAddonLifecycle.AddonEventDelegate(this.OnAddonEvent);
        foreach (var eventType in AllowedEvents)
        {
            this.addonLifecycle.RegisterListener(eventType, name, this.internalAddonEventDelegate);
            this.registeredCallbacks[eventType] = [];
        }
    }

    public void Dispose()
    {
        this.addonLifecycle.UnregisterListener(this.internalAddonEventDelegate);
        this.internalRegisteredEventHandlers.ForEach(this.eventManager.RemoveEvent);
    }

    public void AddHandler(AddonEvent eventType, Callback callback)
    {
        this.registeredCallbacks[eventType].Add(callback);
    }

    public void AddEventHandler(ComponentSelector component, AddonEventType eventType, Callback callback)
    {
        this.eventRegistrations.Add(new(component, eventType, callback));
    }

    private void OnAddonEvent(AddonEvent type, AddonArgs args)
    {
        var addon = (TAddon*)args.Addon;
        switch (type)
        {
            case AddonEvent.PostSetup:
                this.InitEvents(addon, args);
                break;
            case AddonEvent.PreFinalize:
                this.CleanupEvents();
                break;
        }

        this.registeredCallbacks[type].ForEach(listener => listener.Invoke(addon));
    }

    private void InitEvents(TAddon* addon, AddonArgs args)
    {
        this.internalRegisteredEventHandlers.ForEach(this.eventManager.RemoveEvent);
        this.internalRegisteredEventHandlers.Clear();
        this.eventRegistrations.ForEach((Action<EventRegistration<TAddon>>)(registration =>
        {
            var reg = registration;
            var handle = this.eventManager.AddEvent(args.Addon, registration.GetComponent(addon), registration.EventType, (IAddonEventManager.AddonEventHandler)((_, addonPtr, _) => reg.Callback.Invoke((TAddon*)addonPtr)));
            if (handle != null)
            {
                this.internalRegisteredEventHandlers.Add(handle);
            }
            else
            {
                Plugin.Log.Error("Failed to add event handler");
            }
        }));
    }

    private void CleanupEvents()
    {
        this.internalRegisteredEventHandlers.ForEach(this.eventManager.RemoveEvent);
        this.internalRegisteredEventHandlers.Clear();
    }
}
