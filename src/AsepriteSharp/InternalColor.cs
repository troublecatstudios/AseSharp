using AsepriteSharp.Abstractions;

namespace AsepriteSharp {
    public struct InternalColor : IColor {
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

        public InternalColor(float r = 0f, float g = 0f, float b = 0f, float a = 0f) {
            _r = r;
            _g = g;
            _b = b;
            _a = a;
        }

        public InternalColor(IColor color) : this(color.r, color.g, color.b, color.a) { }

        // Adds two colors together. Each component is added separately.
        public static InternalColor operator +(InternalColor a, IColor b) { return new InternalColor(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a); }

        // Subtracts color /b/ from color /a/. Each component is subtracted separately.
        public static InternalColor operator -(InternalColor a, IColor b) { return new InternalColor(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a); }

        // Multiplies two colors together. Each component is multiplied separately.
        public static InternalColor operator *(InternalColor a, IColor b) { return new InternalColor(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a); }

        // Multiplies color /a/ by the float /b/. Each color component is scaled separately.
        public static InternalColor operator *(InternalColor a, float b) { return new InternalColor(a.r * b, a.g * b, a.b * b, a.a * b); }

        // Multiplies color /a/ by the float /b/. Each color component is scaled separately.
        public static InternalColor operator *(float b, InternalColor a) { return new InternalColor(a.r * b, a.g * b, a.b * b, a.a * b); }

        // Divides color /a/ by the float /b/. Each color component is scaled separately.
        public static InternalColor operator /(InternalColor a, float b) { return new InternalColor(a.r / b, a.g / b, a.b / b, a.a / b); }

        public static bool operator ==(InternalColor lhs, IColor rhs) {
            // Returns false in the presence of NaN values.
            return lhs == rhs;
        }

        public static bool operator !=(InternalColor lhs, IColor rhs) {
            // Returns true in the presence of NaN values.
            return !(lhs == rhs);
        }

        // used to allow Colors to be used as keys in hash tables
        public override int GetHashCode() {
            return r.GetHashCode() ^ (g.GetHashCode() << 2) ^ (b.GetHashCode() >> 2) ^ (a.GetHashCode() >> 1);
        }

        // also required for being able to use Colors as keys in hash tables
        public override bool Equals(object? other) {
            if (other == null) return false;
            if (!(other is IColor)) return false;

            return Equals((IColor)other);
        }

        public bool Equals(IColor other) {
            return r.Equals(other.r) && g.Equals(other.g) && b.Equals(other.b) && a.Equals(other.a);
        }
    }
}
