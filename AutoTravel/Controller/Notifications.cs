using AutoTravel.Travel;
using Dalamud.Interface.ImGuiNotification;
using Dalamud.Plugin.Services;

namespace AutoTravel.Controller;

internal class Notifications(INotificationManager notificationManager)
{
    private readonly INotificationManager notificationManager = notificationManager;

    internal void Init(Conductor conductor)
    {
        conductor.OnTravelEnd += this.OnTravelEnd;
        conductor.OnTravelFail += this.OnTravelError;
    }

    internal void OnTravelError(string message)
    {
        this.notificationManager.AddNotification(new Notification()
        {
            Content = "Auto travel error: " + message,
            Type = NotificationType.Error,
        });
    }

    private void OnTravelEnd()
    {
        this.notificationManager.AddNotification(new Notification()
        {
            Content = "Auto travel finished",
            Type = NotificationType.Info,
        });
    }
}
