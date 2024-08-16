using System;

using Dalamud.Configuration;

namespace AutoTravel;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool ButtonInGameShow { get; set; } = true;

    public bool ButtonInGameScaleWithUi { get; set; } = true;

    public int ButtonInGameScale { get; set; } = 100;

    public bool ButtonInLobbyCopySettings { get; set; } = true;

    public bool ButtonInLobbyScaleWithUi { get; set; } = true;

    public int ButtonInLobbyScale { get; set; } = 100;

    public bool RetryCongestedDataCenter { get; set; } = false;

    public float ScaleButton(int index, float uiScale)
    {
        if (this.ButtonInLobbyCopySettings)
        {
            index = -1;
        }

        var scale = index == -1 ? this.ButtonInGameScale : this.ButtonInLobbyScale;
        var scaleWithUi = index == -1 ? this.ButtonInGameScaleWithUi : this.ButtonInLobbyScaleWithUi;
        return scale / 100f * (scaleWithUi ? uiScale : 1f);
    }

    // the below exist just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
