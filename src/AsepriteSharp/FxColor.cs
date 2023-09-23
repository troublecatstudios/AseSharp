using AsepriteSharp.Abstractions;
using System.Drawing;

namespace AsepriteSharp.NetCore {
    public struct FxColor : IColor {
        private const float ColorMax = 255f;
        private float _r;
        private float _g;
        private float _b;
        private float _a;

        public float R { get => _r; set => _r = value; }
        public float r { get => _r; set => _r = value; }
        public float G { get => _g; set => _g = value; }
        public float g { get => _g; set => _g = value; }
        public float B { get => _b; set => _b = value; }
        public float b { get => _b; set => _b = value; }
        public float A { get => _a; set => _a = value; }
        public float a { get => _a; set => _a = value; }

        public FxColor(float r = 0f, float g = 0f, float b = 0f, float a = 0f) {
            _r = r;
            _g = g;
            _b = b;
            _a = a;
        }

        public FxColor(IColor color) : this(color.r, color.g, color.b, color.a) { }
        public FxColor(Color color) : this(color.R / ColorMax, color.G / ColorMax, color.B / ColorMax, color.A / ColorMax) { }

        public static implicit operator Color(FxColor color) => Color.FromArgb((int)(color.a * ColorMax), (int)(color.r * ColorMax), (int)(color.g * ColorMax), (int)(color.b * ColorMax));
        public static implicit operator InternalColor(FxColor color) => new(color);
        public static implicit operator FxColor(Color color) => new(color);
    }
}
