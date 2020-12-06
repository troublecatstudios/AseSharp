namespace AsefileSharp.PixelFormats {
    public class GrayscalePixel : Pixel {
        public byte[] Color { get; private set; }

        public GrayscalePixel(Frame frame, byte[] color) : base(frame) {
            Color = color;
        }

        public override InternalColor GetColor() {
            float value = (float)Color[0] / 255;
            float alpha = (float)Color[1] / 255;

            return new InternalColor(value, value, value, alpha);
        }
    }
}
