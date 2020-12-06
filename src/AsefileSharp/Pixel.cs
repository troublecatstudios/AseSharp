namespace AsefileSharp {
    public abstract class Pixel {
        internal static InternalColor _magenta = new InternalColor(1, 0, 1, 1);
        protected Frame Frame = null;
        public abstract InternalColor GetColor();

        public Pixel(Frame frame) {
            Frame = frame;
        }
    }
}

