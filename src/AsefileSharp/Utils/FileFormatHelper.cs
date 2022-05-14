namespace AsefileSharp.Utils {
    internal static class FileFormatHelper {
        public static bool IsBitSet(int b, int pos) {
            return (b & (1 << pos)) != 0;
        }
    }
}
