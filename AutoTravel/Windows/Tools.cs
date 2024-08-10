using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Plugin.Services;

namespace AutoTravel.Windows;

internal class Tools(ITextureProvider textureProvider)
{
    private readonly ITextureProvider textureProvider = textureProvider;

    internal IDalamudTextureWrap GetIconWrap(int inx)
    {
        return this.textureProvider.GetFromGameIcon(inx).GetWrapOrEmpty();
    }
}
