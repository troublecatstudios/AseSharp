using AsefileSharp.Abstractions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AsefileSharp.NetCore {
    public class FxTexture : ITexture {
        private readonly Bitmap _bitmap;
        private readonly Graphics _graphics;
        private string _name;

        public FxTexture(int width, int height) {
            _bitmap = new Bitmap(width, height);
            _graphics = Graphics.FromImage(_bitmap);
            _graphics.Clear(Color.Transparent);
        }

        public int Height { get => _bitmap.Height; set => _ = value; }
        public int height { get => _bitmap.Height; set => _ = value; }
        public int Width { get => _bitmap.Width; set => _ = value; }
        public int width { get => _bitmap.Width; set => _ = value; }
        public string name { get => _name; set => _name = value; }

        public void Apply() {
            _graphics.Flush();
        }

        public InternalColor GetPixel(int x, int y) {
            var color = _bitmap.GetPixel(x, y);
            return new FxColor(color);
        }

        public IEnumerable<InternalColor> GetPixels() {
            var pixels = new List<FxColor>();
            for (var y = 0; y < _bitmap.Height; y++) {
                for (var x = 0; x < _bitmap.Width; x++) {
                    var color = _bitmap.GetPixel(x, y);
                    pixels.Add(new FxColor(color));
                }
            }
            return pixels.Cast<InternalColor>();
        }

        public void SetPixel(int x, int y, IColor color) {
            _bitmap.SetPixel(x, y, new FxColor(color));
        }

        public void SetPixels(IEnumerable<IColor> colors) {
            for (var i = 0; i < colors.Count(); i++) {
                var y = i / Width;
                var x = i % Width;
                var color = colors.ElementAt(i);
                SetPixel(x, y, color);
            }
        }

        public void SetPixels(int startX, int startY, int endX, int endY, IEnumerable<IColor> colors) {
            throw new NotImplementedException();
        }
    }
}
