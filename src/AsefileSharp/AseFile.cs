using AsefileSharp.Abstractions;
using AsefileSharp.Chunks;
using AsefileSharp.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace AsefileSharp {

    // See file specs here: https://github.com/aseprite/aseprite/blob/master/docs/ase-file-specs.md

    public class AseFile {
        public Header Header { get; private set; }
        public List<Frame> Frames { get; private set; }

        private readonly Dictionary<Type, Chunk> chunkCache = new Dictionary<Type, Chunk>();
        private readonly ITextureBuilder _builder;
        private readonly Texture2DBlender _blender;

        public AseFile(Stream stream, ITextureBuilder builder, Texture2DBlender blender = null) {
            _builder = builder;
            _blender = blender ?? new Texture2DBlender(_builder);
            BinaryReader reader = new BinaryReader(stream);
            byte[] header = reader.ReadBytes(128);

            Header = new Header(header);
            Frames = new List<Frame>();

            while (reader.BaseStream.Position < reader.BaseStream.Length) {
                Frames.Add(new Frame(this, reader));
            }
        }


        public List<T> GetChunks<T>() where T : Chunk {
            List<T> chunks = new List<T>();

            for (int i = 0; i < Frames.Count; i++) {
                List<T> cs = Frames[i].GetChunks<T>();

                chunks.AddRange(cs);
            }

            return chunks;
        }

        public T GetChunk<T>() where T : Chunk {
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

        public ITexture[] GetFrames() {
            var frames = new List<ITexture>();

            for (int i = 0; i < Frames.Count; i++) {
                frames.Add(GetFrame(i));
            }

            return frames.ToArray();
        }


        public ITexture[] GetLayersAsFrames() {
            var frames = new List<ITexture>();
            var layers = GetChunks<LayerChunk>();

            for (int i = 0; i < layers.Count; i++) {
                var layerFrames = GetLayerTexture(i, layers[i]);

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

        public List<ITexture> GetLayerTexture(int layerIndex, LayerChunk layer) {
            var textures = new List<ITexture>();

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

                    var tex = GetTextureFromCel(cels[i]);
                    tex.name = $"{layer.LayerName}_{i}";
                    textures.Add(tex);
                }
            }

            return textures;
        }

        public ITexture GetFrame(int index) {
            var frame = Frames[index];
            var texture = _builder.CreateTexture(Header.Width, Header.Height, setTransparent: true);
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

                var celTex = GetTextureFromCel(cels[i]);

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

            texture.name = $"{index}";

            return texture;
        }

        public ITexture GetTextureFromCel(CelChunk cel) {
            int canvasWidth = Header.Width;
            int canvasHeight = Header.Height;

            var texture = _builder.CreateTexture(canvasWidth, canvasHeight, setTransparent: true);
            InternalColor[] colors = new InternalColor[canvasWidth * canvasHeight];

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
                        int index = (canvasHeight - 1 - y) * canvasWidth + x;
                        colors[index] = cel.RawPixelData[pixelIndex].GetColor();
                    }

                    ++pixelIndex;
                }
            }

            texture.SetPixels(0, 0, canvasWidth, canvasHeight, colors.Cast<IColor>());
            texture.Apply();

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

        public MetaData[] GetMetaData(int spritePivotX, int spritePivotY, int pixelsPerUnit) {
            var metadatas = new Dictionary<int, MetaData>();

            for (int index = 0; index < Frames.Count; index++) {
                var layers = GetChunks<LayerChunk>();
                var cels = Frames[index].GetChunks<CelChunk>();

                cels.Sort((ca, cb) => ca.LayerIndex.CompareTo(cb.LayerIndex));

                for (int i = 0; i < cels.Count; i++) {
                    var layerIndex = cels[i].LayerIndex;
                    var layer = layers[layerIndex];
                    if (!layer.LayerName.StartsWith(MetaData.MetaDataChar)) //read only metadata layer
                        continue;

                    if (!metadatas.ContainsKey(layerIndex))
                        metadatas[layerIndex] = new MetaData(layer.LayerName);
                    var metadata = metadatas[layerIndex];

                    var cel = cels[i];
                    var centerX = 0f;
                    var centerY = 0f;
                    var pixelCount = 0;

                    for (int y = 0; y < cel.Height; ++y) {
                        for (int x = 0; x < cel.Width; ++x) {
                            int texX = cel.X + x;
                            int texY = -(cel.Y + y) + Header.Height - 1;
                            var col = cel.RawPixelData[x + y * cel.Width];
                            if (col.GetColor().a > 0.1f) {
                                centerX += texX;
                                centerY += texY;
                                pixelCount++;
                            }
                        }
                    }

                    if (pixelCount > 0) {
                        centerX /= pixelCount;
                        centerY /= pixelCount;
                        var pivotX = spritePivotX * Header.Width;
                        var pivotY = spritePivotY * Header.Height;

                        var posWorldX = (centerX - pivotX) / pixelsPerUnit + 1 * 0.5f / pixelsPerUnit;
                        var posWorldY = (centerY - pivotY) / pixelsPerUnit + 1 * 0.5f / pixelsPerUnit;

                        metadata.Transforms.Add(index, (posWorldX, posWorldY));
                    }
                }
            }
            return metadatas.Values.ToArray();
        }

        public ITexture GetTextureAtlas() {
            var frames = GetFrames();
            var atlas = _builder.CreateTexture(Header.Width * frames.Length, Header.Height, setTransparent: true);
            var col = 0;
            var row = 0;

            foreach (ITexture frame in frames) {
                var x = col * Header.Width;
                var y = atlas.height - (row + 1) * Header.Height;
                var width = Header.Width;
                var height = Header.Height;
                atlas.SetPixels((int)x, (int)y, (int)width, (int)height, frame.GetPixels().Cast<IColor>());
                atlas.Apply();
                col++;
            }

            return atlas;
        }
    }

}
