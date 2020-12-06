using System.Collections.Generic;

namespace AsefileSharp.Abstractions {
    public interface ITexture {
        int Height { get; set; }
        int Width { get; set; }

        int height { get; set; }
        int width { get; set; }

        string name { get; set; }

        InternalColor GetPixel(int x, int y);
        IEnumerable<InternalColor> GetPixels();
        void SetPixel(int x, int y, IColor color);
        void SetPixels(IEnumerable<IColor> colors);
        void SetPixels(int startX, int startY, int endX, int endY, IEnumerable<IColor> colors);
        void Apply();
    }
}
