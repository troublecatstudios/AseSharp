using System;
using System.Runtime.CompilerServices;

namespace AsepriteSharp.IntegrationTests {
    internal static class ProjectSourcePath {
        private const string myRelativePath = nameof(ProjectSourcePath) + ".cs";
        private static string? lazyValue;
        public static string Value => lazyValue ??= calculatePath();

        private static string calculatePath() {
            string pathName = GetSourceFilePathName();
            if (!pathName.EndsWith(myRelativePath, StringComparison.Ordinal)) {
                throw new Exception($"Bad project source path '{pathName}'!");
            }
            return pathName.Substring(0, pathName.Length - myRelativePath.Length);
        }

        public static string GetSourceFilePathName([CallerFilePath] string? callerFilePath = null) => callerFilePath ?? "";
    }
}
