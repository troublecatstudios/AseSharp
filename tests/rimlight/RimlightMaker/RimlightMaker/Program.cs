using AsepriteSharp;
using AsepriteSharp.NetCore;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace RimlightMaker {
    internal class Program {
        private static FxColor whitePx = new FxColor(1, 1, 1, 1);

        private static void Main(string[] args) {
            var asepriteFilePath = @"D:\src\troublecat\AsefileSharp\tests\resources\Ode-Sit-Cuffed.aseprite";
            using var fileStream = new FileStream(asepriteFilePath, FileMode.Open, FileAccess.Read);
            var aseFile = new AsepriteFile(fileStream);
            var texture = aseFile.GetTexturePixels();

            var bitmap = new Bitmap(texture.Width, texture.Height);
            var idx = 0;
            foreach (var px in texture.Pixels) {
                var y = idx / texture.Width;
                var x = idx % texture.Width;

                var top = y > 0 && texture.GetPixel(x, y - 1).A == 0;
                var bottom = y < texture.Height - 1 && texture.GetPixel(x, y + 1).A == 0;
                var left = x > 0 && texture.GetPixel(x - 1, y).A == 0;
                var right = x < texture.Width - 1 && texture.GetPixel(x + 1, y).A == 0;

                if (px.A > 0 && (top || bottom || left || right)) {
                    bitmap.SetPixel(x, y, whitePx);
                }
                idx++;
            }
            bitmap.Save(@"D:\src\troublecat\AsefileSharp\tests\resources\output.png", ImageFormat.Png);
        }
    }
}
