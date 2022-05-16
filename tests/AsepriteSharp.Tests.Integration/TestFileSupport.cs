using AsepriteSharp.Chunks;
using FluentAssertions;
using System.IO;
using System.Linq;
using Xunit;

namespace AsepriteSharp.IntegrationTests {
    public class TestFileSupport {
        private const string ResourcesDirectory = "./resources";

        [Theory]
        [InlineData("Abberline-32", 2)]
        [InlineData("Ode-32", 139)]
        [InlineData("Ode-Sit-Cuffed", 12)]
        [InlineData("SlicesBasic", 1)]
        public void WhenLoadingBasicAsepriteFile_ItShouldHaveTheRightNumberOfFrames(string resource, int expectedFrameCount) {
            var path = GetResourcePath(resource);

            // try loading the resource
            var file = LoadFile(path);
            file.Frames.Should().HaveCount(expectedFrameCount);
        }

        [Theory]
        [InlineData("Abberline-32", 2)]
        [InlineData("Ode-32", 6)]
        [InlineData("Ode-Sit-Cuffed", 5)]
        [InlineData("SlicesBasic", 3)]
        public void WhenLoadingBasicAsepriteFile_ItShouldHaveTheRightNumberOfLayers(string resource, int expectedLayerCount) {
            var path = GetResourcePath(resource);

            // try loading the resource
            var file = LoadFile(path);
            file.Layers.Should().HaveCount(expectedLayerCount);
        }

        [Theory]
        [InlineData("SlicesBasic", "Battery_Normal", "Battery_Damaged", "Battery_Dead", "ControlRoom")]
        [InlineData("SlicesWithPivots", "Battery_Normal", "Battery_Damaged", "Battery_Dead", "ControlRoom")]
        public void WhenLoadingSlices_ItShouldIncludeAllTheSlices(string resource, params string[] expectedSliceNames) {
            var path = GetResourcePath(resource);

            // try loading the resource
            var file = LoadFile(path);
            var firstFrame = file.Frames[0];
            var slices = firstFrame.GetChunks<SliceChunk>();

            slices.Should().HaveCount(expectedSliceNames.Length);
            foreach (var expectedName in expectedSliceNames) {
                slices.Where(s => s.SliceName == expectedName).Should().HaveCount(1);
            }
        }

        [Theory]
        [InlineData("SlicesBasic", "Battery_Normal", 0, 0)]
        [InlineData("SlicesBasic", "Battery_Damaged", 0, 0)]
        [InlineData("SlicesBasic", "Battery_Dead", 0, 0)]
        [InlineData("SlicesBasic", "ControlRoom", 0, 0)]
        [InlineData("SlicesWithPivots", "Battery_Normal", 8, 12)]
        [InlineData("SlicesWithPivots", "Battery_Damaged", 8, 12)]
        [InlineData("SlicesWithPivots", "Battery_Dead", 8, 12)]
        [InlineData("SlicesWithPivots", "ControlRoom", 32, 32)]
        public void WhenLoadingSlices_ItShouldReadThePivotComponentsCorrectly(string resource, string expectedSliceName, int expectedPivotX, int expectedPivotY) {
            var path = GetResourcePath(resource);

            // try loading the resource
            var file = LoadFile(path);
            var firstFrame = file.Frames[0];
            var slices = firstFrame.GetChunks<SliceChunk>();
            var slice = slices.Where(s => s.SliceName.Equals(expectedSliceName)).FirstOrDefault();
            slice.Should().NotBeNull();
            slice.Entries.Should().HaveCount(1);
            slice.Entries.First().PivotX.Should().Be(expectedPivotX);
            slice.Entries.First().PivotY.Should().Be(expectedPivotY);
        }

        private AsepriteFile LoadFile(string path) {
            using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var file = new AsepriteFile(fileStream);
            return file;
        }

        private string GetResourcePath(string resourceName) {
            // get the root directory
            var resourcesAbsolutePath = Path.Combine(ProjectSourcePath.Value, "../", ResourcesDirectory);
            if (!resourceName.EndsWith(".aseprite")) {
                resourceName += ".aseprite";
            }
            return Path.Combine(resourcesAbsolutePath, resourceName);
        }
    }
}