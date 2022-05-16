using AsepriteSharp.Abstractions;
using AsepriteSharp.Chunks;
using AsepriteSharp.Utils;
using System;
using System.Collections.Generic;
using System.IO;


namespace AsepriteSharp {

    /// <summary>
    /// Represents a *.ase or *.aseprite file
    /// </summary>
    /// <remarks>
    /// See file specs here: https://github.com/aseprite/aseprite/blob/master/docs/ase-file-specs.md
    /// </remarks>
    public class AsepriteFile {
        private readonly Dictionary<Type, AsepriteFileChunk> chunkCache = new Dictionary<Type, AsepriteFileChunk>();
        private readonly Texture2DBlender _blender;

        public Header Header { get; private set; }
        public List<Frame> Frames { get; private set; }
        public List<LayerChunk> Layers => GetChunks<LayerChunk>();

        public AsepriteFile(Stream stream, Texture2DBlender blender = null) {
            _blender = blender ?? new Texture2DBlender();
            BinaryReader reader = new BinaryReader(stream);
            byte[] header = reader.ReadBytes(128);

            Header = new Header(header);
            Frames = new List<Frame>();

            while (reader.BaseStream.Position < reader.BaseStream.Length) {
                Frames.Add(new Frame(this, reader));
            }
        }

        public List<T> GetChunks<T>() where T : AsepriteFileChunk {
            List<T> chunks = new List<T>();

            for (int i = 0; i < Frames.Count; i++) {
                List<T> cs = Frames[i].GetChunks<T>();

                chunks.AddRange(cs);
            }

            return chunks;
        }

        public T GetChunk<T>() where T : AsepriteFileChunk {
            if (!chunkCache.ContainsKey(typeof(T))) {
                for (int i = 0; i < Frames.Count; i++) {
                    List<T> cs = Frames[i].GetChunks<T>();

                    if (cs.Count > 0) {
                        chunkCache.Add(typeof(T), cs[0]);
                        break;
                    }
                }
            }

            return (T)chunkCache[typeof(T)];
        }

        public PixelBucket[] GetFrames() {
            var frames = new List<PixelBucket>();

            for (int i = 0; i < Frames.Count; i++) {
                frames.Add(GetFramePixels(i));
            }

            return frames.ToArray();
        }


        public PixelBucket[] GetLayersAsFrames() {
            var frames = new List<PixelBucket>();
            var layers = GetChunks<LayerChunk>();

            for (int i = 0; i < layers.Count; i++) {
                var layerFrames = GetLayerPixels(i, layers[i]);

                if (layerFrames.Count > 0) {
                    frames.AddRange(layerFrames);
                }
            }

            return frames.ToArray();
        }

        private LayerChunk GetParentLayer(LayerChunk layer) {
            if (layer.LayerChildLevel == 0)
                return null;

            var layers = GetChunks<LayerChunk>();
            var index = layers.IndexOf(layer);

            if (index < 0)
                return null;

            for (int i = index - 1; i > 0; i--) {
                if (layers[i].LayerChildLevel == layer.LayerChildLevel - 1)
                    return layers[i];
            }

            return null;
        }

        public List<PixelBucket> GetLayerPixels(int layerIndex, LayerChunk layer) {
            var textures = new List<PixelBucket>();

            for (int frameIndex = 0; frameIndex < Frames.Count; frameIndex++) {
                var frame = Frames[frameIndex];
                var cels = frame.GetChunks<CelChunk>();

                for (int i = 0; i < cels.Count; i++) {
                    if (cels[i].LayerIndex != layerIndex)
                        continue;

                    var blendMode = layer.BlendMode;
                    var opacity = InternalMath.Min(layer.Opacity / 255f, cels[i].Opacity / 255f);

                    var visibility = layer.Visible;

                    var parent = GetParentLayer(layer);
                    while (parent != null) {
                        visibility &= parent.Visible;
                        if (visibility == false)
                            break;

                        parent = GetParentLayer(parent);
                    }

                    if (visibility == false || layer.LayerType == LayerType.Group)
                        continue;

                    var tex = GetCelPixels(cels[i]);
                    tex.Name = $"{layer.LayerName}_{i}";
                    textures.Add(tex);
                }
            }

            return textures;
        }

        public PixelBucket GetFramePixels(int index) {
            var frame = Frames[index];
            var texture = new PixelBucket(Header.Width, Header.Height);
            var layers = GetChunks<LayerChunk>();
            var cels = frame.GetChunks<CelChunk>();

            cels.Sort((ca, cb) => ca.LayerIndex.CompareTo(cb.LayerIndex));

            for (int i = 0; i < cels.Count; i++) {
                var layer = layers[cels[i].LayerIndex];
                if (layer.LayerName.StartsWith("@")) //ignore metadata layer
                    continue;

                var blendMode = layer.BlendMode;
                float opacity = InternalMath.Min(layer.Opacity / 255f, cels[i].Opacity / 255f);

                bool visibility = layer.Visible;


                var parent = GetParentLayer(layer);
                while (parent != null) {
                    visibility &= parent.Visible;
                    if (visibility == false)
                        break;

                    parent = GetParentLayer(parent);
                }

                if (visibility == false || layer.LayerType == LayerType.Group)
                    continue;

                var celTex = GetCelPixels(cels[i]);

                switch (blendMode) {
                    case LayerBlendMode.Normal: texture = _blender.Normal(texture, celTex); break;
                    case LayerBlendMode.Multiply: texture = _blender.Multiply(texture, celTex, opacity); break;
                    case LayerBlendMode.Screen: texture = _blender.Screen(texture, celTex); break;
                    case LayerBlendMode.Overlay: texture = _blender.Overlay(texture, celTex); break;
                    case LayerBlendMode.Darken: texture = _blender.Darken(texture, celTex); break;
                    case LayerBlendMode.Lighten: texture = _blender.Lighten(texture, celTex); break;
                    case LayerBlendMode.ColorDodge: texture = _blender.ColorDodge(texture, celTex); break;
                    case LayerBlendMode.ColorBurn: texture = _blender.ColorBurn(texture, celTex); break;
                    case LayerBlendMode.HardLight: texture = _blender.HardLight(texture, celTex); break;
                    case LayerBlendMode.SoftLight: texture = _blender.SoftLight(texture, celTex); break;
                    case LayerBlendMode.Difference: texture = _blender.Difference(texture, celTex); break;
                    case LayerBlendMode.Exclusion: texture = _blender.Exclusion(texture, celTex); break;
                    case LayerBlendMode.Hue: texture = _blender.Hue(texture, celTex); break;
                    case LayerBlendMode.Saturation: texture = _blender.Saturation(texture, celTex); break;
                    case LayerBlendMode.Color: texture = _blender.Color(texture, celTex); break;
                    case LayerBlendMode.Luminosity: texture = _blender.Luminosity(texture, celTex); break;
                    case LayerBlendMode.Addition: texture = _blender.Addition(texture, celTex); break;
                    case LayerBlendMode.Subtract: texture = _blender.Subtract(texture, celTex); break;
                    case LayerBlendMode.Divide: texture = _blender.Divide(texture, celTex); break;
                }
            }

            texture.Name = $"{index}";

            return texture;
        }

        private PixelBucket GetCelPixels(CelChunk cel) {
            int canvasWidth = Header.Width;
            int canvasHeight = Header.Height;

            var texture = new PixelBucket(canvasWidth, canvasHeight);

            int pixelIndex = 0;
            int celXEnd = cel.Width + cel.X;
            int celYEnd = cel.Height + cel.Y;


            for (int y = cel.Y; y < celYEnd; y++) {
                if (y < 0 || y >= canvasHeight) {
                    pixelIndex += cel.Width;
                    continue;
                }

                for (int x = cel.X; x < celXEnd; x++) {
                    if (x >= 0 && x < canvasWidth) {
                        var color = cel.RawPixelData[pixelIndex].GetColor();
                        texture.SetPixel(x, y, color);
                    }

                    ++pixelIndex;
                }
            }

            return texture;
        }

        public FrameTag[] GetAnimations() {
            var tagChunks = GetChunks<FrameTagsChunk>();

            var animations = new List<FrameTag>();

            foreach (FrameTagsChunk tagChunk in tagChunks) {
                foreach (FrameTag tag in tagChunk.Tags) {
                    animations.Add(tag);
                }
            }

            return animations.ToArray();
        }

        public PixelBucket GetTexturePixels() {
            var frames = GetFrames();
            var atlas = new PixelBucket(Header.Width * frames.Length, Header.Height);
            var col = 0;
            var row = 0;

            foreach (var frame in frames) {
                var startX = col * Header.Width;
                var startY = atlas.Height - (row + 1) * Header.Height;
                var width = Header.Width;
                var height = Header.Height;

                int idx = 0;
                foreach (var px in frame.Pixels) {
                    var x = idx % width;
                    var y = idx / width;
                    atlas.SetPixel(startX + x, startY + y, px);
                    idx++;
                }
                col++;
            }

            return atlas;
        }
    }

}
