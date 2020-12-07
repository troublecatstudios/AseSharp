namespace AsefileSharp.Abstractions {
    public class PixelBucket {
        private readonly InternalColor[] _pixels;

        public PixelBucket(int width, int height) {
            _pixels = new InternalColor[width * height];
            Width = width;
            Height = height;
        }

        public InternalColor[] Pixels => _pixels;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public string Name { get; set; }

        public void SetPixel(int x, int y, IColor color) {
            var index = CalculateIndex(x, y);
            _pixels[index] = new InternalColor(color);
        }

        public InternalColor GetPixel(int x, int y) {
            var index = CalculateIndex(x, y);
            return _pixels[index];
        }

        protected virtual int CalculateIndex(int x, int y) {
            return y * Width + x;
        }
    }
}
