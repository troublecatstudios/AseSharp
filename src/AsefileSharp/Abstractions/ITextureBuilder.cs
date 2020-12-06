namespace AsefileSharp.Abstractions {
    public interface ITextureBuilder {
        ITexture CreateTexture(int width, int height, bool setTransparent = false);
    }
}
