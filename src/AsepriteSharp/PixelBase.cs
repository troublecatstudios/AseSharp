namespace AsepriteSharp {
    public abstract class PixelBase {
        internal static InternalColor _magenta = new InternalColor(1, 0, 1, 1);
        protected Frame Frame = null;

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <returns></returns>
        public abstract InternalColor GetColor();

        public PixelBase(Frame frame) {
            Frame = frame;
        }
    }
}

