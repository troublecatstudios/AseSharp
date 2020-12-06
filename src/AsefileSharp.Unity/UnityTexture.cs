using AsefileSharp.Abstractions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AsefileSharp.Unity {
    public class UnityTexture : ITexture {
        private readonly Texture2D _texture;

        public UnityTexture(int width, int height) {
            _texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        }

        public int Height { get => _texture.height; set => _ = value; }
        public int height { get => _texture.height; set => _ = value; }
        public int Width { get => _texture.width; set => _ = value; }
        public int width { get => _texture.width; set => _ = value; }
        public string name { get => _texture.name; set => _texture.name = value; }

        public void Apply() {
            _texture.Apply();
        }

        public InternalColor GetPixel(int x, int y) {
            var color = _texture.GetPixel(x, y);
            return new InternalColor(color.r, color.g, color.b, color.a);
        }

        public IEnumerable<InternalColor> GetPixels() {
            var pixels = _texture.GetPixels();
            return pixels.Select(px => new InternalColor(px.r, px.g, px.b, px.a));
        }

        public void SetPixel(int x, int y, IColor color) {
            _texture.SetPixel(x, y, new Color(color.r, color.g, color.b, color.a));
        }

        public void SetPixels(IEnumerable<IColor> colors) {
            var pixels = colors.Select(c => new Color(c.r, c.g, c.b, c.a));
            _texture.SetPixels(pixels.ToArray());
        }

        public void SetPixels(int startX, int startY, int endX, int endY, IEnumerable<IColor> colors) {
            var pixels = colors.Select(c => new Color(c.r, c.g, c.b, c.a));
            _texture.SetPixels(startX, startY, endX, endY, pixels.ToArray());
        }
    }
}
