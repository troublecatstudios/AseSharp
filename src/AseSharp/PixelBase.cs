namespace AseSharp {
    public abstract class PixelBase {
        internal static InternalColor _magenta = new(1, 0, 1, 1);
        protected Frame? Frame;

        public PixelBase(Frame frame) {
            Frame = frame;
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <returns></returns>
        public abstract InternalColor GetColor();
    }
}

