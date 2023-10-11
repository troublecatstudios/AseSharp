using System;

namespace AseSharp.Chunks {
    [Flags]
    public enum SliceFlags : ushort {
        IsNinePatch = 1,
        HasPivot = 2
    }
}
