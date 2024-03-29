﻿using AseSharp.Chunks;

namespace AseSharp.PixelFormats {
    public class IndexedPixel : PixelBase {
        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public byte Index { get; private set; }

        public IndexedPixel(Frame frame, byte index) : base(frame) {
            Index = index;
        }

        public override InternalColor GetColor() {
            PaletteChunk? palette = Frame?.File?.GetChunk<PaletteChunk>();

            if (palette != null)
                return palette.GetColor(Index);
            else
                return _magenta;
        }
    }
}
