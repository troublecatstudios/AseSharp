using AsefileSharp.Abstractions;

namespace AsefileSharp.NetCore {
    public class FxTextureBuilder : ITextureBuilder {
        public ITexture CreateTexture(int width, int height, bool setTransparent = false) {
            var texture = new FxTexture(width, height);
            return texture;
        }
    }
}
