using System.IO;

namespace AsepriteSharp.Chunks {
    public class LinkedCelChunk : CelChunk {
        private AsepriteFile file = null;
        private CelChunk linkedCelChunk = null;

        /// <summary>
        /// Gets the linked cel.
        /// </summary>
        /// <value>
        /// The linked cel.
        /// </value>
        public CelChunk LinkedCel {
            get {
                if (linkedCelChunk == null) {
                    linkedCelChunk = file.Frames[FramePosition].GetCelChunk<CelChunk>(LayerIndex);
                }

                return linkedCelChunk;
            }
        }

        /// <summary>
        /// Gets the frame position.
        /// </summary>
        /// <value>
        /// The frame position.
        /// </value>
        public ushort FramePosition { get; private set; }

        public override ushort Width { get { return LinkedCel.Width; } }
        public override ushort Height { get { return LinkedCel.Height; } }
        public override PixelBase[] RawPixelData { get { return LinkedCel.RawPixelData; } }

        public LinkedCelChunk(uint length, ushort layerIndex, short x, short y, byte opacity, Frame frame, BinaryReader reader) : base(length, layerIndex, x, y, opacity, CelType.Linked) {
            file = frame.File;

            FramePosition = reader.ReadUInt16();
        }
    }
}
