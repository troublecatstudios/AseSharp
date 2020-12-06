using AsefileSharp.Abstractions;
using System.Linq;

namespace AsefileSharp.Unity {
    public class UnityTextureBuilder : ITextureBuilder {
        public ITexture CreateTexture(int width, int height, bool setTransparent = false) {
            var texture = new UnityTexture(width, height);
            if (setTransparent) {
                var pixels = new UnityColor[width * height];

                for (int i = 0; i < pixels.Length; i++) pixels[i] = UnityEngine.Color.clear;

                texture.SetPixels(pixels.Cast<IColor>());
                texture.Apply();
            }
            return texture;
        }
    }
}
