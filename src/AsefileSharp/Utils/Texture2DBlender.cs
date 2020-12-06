using AsefileSharp.Abstractions;

// See: http://wwwimages.adobe.com/www.adobe.com/content/dam/Adobe/en/devnet/pdf/pdfs/PDF32000_2008.pdf
// Page 333
namespace AsefileSharp.Utils {
    public class Texture2DBlender {
        private readonly ITextureBuilder _builder;

        public Texture2DBlender(ITextureBuilder builder) {
            _builder = builder;
        }

        public ITexture Normal(ITexture baseLayer, ITexture layer) {
            var newLayer = _builder.CreateTexture(baseLayer.width, baseLayer.height);

            for (int x = 0; x < baseLayer.width; x++) {
                for (int y = 0; y < baseLayer.height; y++) {
                    var a = baseLayer.GetPixel(x, y);
                    var b = layer.GetPixel(x, y);


                    var c = new InternalColor();

                    c = ((1f - b.a) * a) + (b.a * b);
                    c.a = a.a + b.a * (1f - a.a);

                    newLayer.SetPixel(x, y, c);
                }
            }

            newLayer.Apply();

            return newLayer;
        }

        public ITexture Multiply(ITexture baseLayer, ITexture layer, float opacity) {
            var newLayer = _builder.CreateTexture(baseLayer.width, baseLayer.height);

            for (int x = 0; x < baseLayer.width; x++) {
                for (int y = 0; y < baseLayer.height; y++) {
                    var a = baseLayer.GetPixel(x, y);
                    var b = layer.GetPixel(x, y);

                    var c = new InternalColor();

                    c.r = (a.r) * (opacity * (1f - b.a * (1f - b.r)));
                    c.g = (a.g) * (opacity * (1f - b.a * (1f - b.g)));
                    c.b = (a.b) * (opacity * (1f - b.a * (1f - b.b)));
                    c.a = a.a + b.a * (1f - a.a);

                    newLayer.SetPixel(x, y, c);
                }
            }

            newLayer.Apply();

            return newLayer;
        }


        public ITexture Screen(ITexture baseLayer, ITexture layer) {
            var newLayer = _builder.CreateTexture(baseLayer.width, baseLayer.height);

            for (int x = 0; x < baseLayer.width; x++) {
                for (int y = 0; y < baseLayer.height; y++) {
                    var a = baseLayer.GetPixel(x, y);
                    var b = layer.GetPixel(x, y);

                    var c = a + b - (a * b);

                    newLayer.SetPixel(x, y, c);
                }
            }

            newLayer.Apply();

            return newLayer;
        }

        public ITexture Overlay(ITexture baseLayer, ITexture layer) {
            var newLayer = _builder.CreateTexture(baseLayer.width, baseLayer.height);

            for (int x = 0; x < baseLayer.width; x++) {
                for (int y = 0; y < baseLayer.height; y++) {
                    var a = baseLayer.GetPixel(x, y);
                    var b = layer.GetPixel(x, y);

                    var c = new InternalColor();


                    if (a.r < 0.5)
                        c.r = 2f * a.r * b.r;
                    else
                        c.r = 1f - 2f * (1f - b.r) * (1f - a.r);

                    if (a.g < 0.5)
                        c.g = 2f * a.g * b.g;
                    else
                        c.g = 1f - 2f * (1f - b.g) * (1f - a.g);

                    if (a.b < 0.5)
                        c.b = 2f * a.b * b.b;
                    else
                        c.b = 1f - 2f * (1f - b.b) * (1f - a.b);

                    c = ((1f - b.a) * a) + (b.a * c);

                    c.a = a.a + b.a * (1f - a.a);



                    newLayer.SetPixel(x, y, c);

                }
            }

            newLayer.Apply();

            return newLayer;
        }

        public ITexture Darken(ITexture baseLayer, ITexture layer) {
            var newLayer = _builder.CreateTexture(baseLayer.width, baseLayer.height);

            for (int x = 0; x < baseLayer.width; x++) {
                for (int y = 0; y < baseLayer.height; y++) {
                    var a = baseLayer.GetPixel(x, y);
                    var b = layer.GetPixel(x, y);

                    var c = new InternalColor();


                    c.r = InternalMath.Min(a.r, b.r);
                    c.g = InternalMath.Min(a.g, b.g);
                    c.b = InternalMath.Min(a.b, b.b);

                    c = ((1f - b.a) * a) + (b.a * c);
                    c.a = a.a + b.a * (1f - a.a);

                    newLayer.SetPixel(x, y, c);
                }
            }

            newLayer.Apply();

            return newLayer;
        }

        public ITexture Lighten(ITexture baseLayer, ITexture layer) {
            var newLayer = _builder.CreateTexture(baseLayer.width, baseLayer.height);

            for (int x = 0; x < baseLayer.width; x++) {
                for (int y = 0; y < baseLayer.height; y++) {
                    var a = baseLayer.GetPixel(x, y);
                    var b = layer.GetPixel(x, y);

                    var c = new InternalColor();

                    c.r = ColorBlends.Lighten(a.r, b.r);
                    c.g = ColorBlends.Lighten(a.g, b.g);
                    c.b = ColorBlends.Lighten(a.b, b.b);

                    c = ((1f - b.a) * a) + (b.a * c);
                    c.a = a.a + b.a * (1f - a.a);

                    newLayer.SetPixel(x, y, c);
                }
            }

            newLayer.Apply();

            return newLayer;
        }

        public ITexture ColorDodge(ITexture baseLayer, ITexture layer) {
            var newLayer = _builder.CreateTexture(baseLayer.width, baseLayer.height);

            for (int x = 0; x < baseLayer.width; x++) {
                for (int y = 0; y < baseLayer.height; y++) {
                    var a = baseLayer.GetPixel(x, y);
                    var b = layer.GetPixel(x, y);

                    var c = new InternalColor();


                    c.r = ColorBlends.ColorDodge(a.r, b.r);
                    c.g = ColorBlends.ColorDodge(a.g, b.g);
                    c.b = ColorBlends.ColorDodge(a.b, b.b);

                    c = ((1f - b.a) * a) + (b.a * c);
                    c.a = a.a + b.a * (1f - a.a);

                    newLayer.SetPixel(x, y, c);
                }
            }

            newLayer.Apply();

            return newLayer;
        }

        public ITexture ColorBurn(ITexture baseLayer, ITexture layer) {
            var newLayer = _builder.CreateTexture(baseLayer.width, baseLayer.height);

            for (int x = 0; x < baseLayer.width; x++) {
                for (int y = 0; y < baseLayer.height; y++) {
                    var a = baseLayer.GetPixel(x, y);
                    var b = layer.GetPixel(x, y);

                    var c = new InternalColor();

                    c.r = ColorBlends.ColorBurn(a.r, b.r);
                    c.g = ColorBlends.ColorBurn(a.g, b.g);
                    c.b = ColorBlends.ColorBurn(a.b, b.b);

                    c = ((1f - b.a) * a) + (b.a * c);
                    c.a = a.a + b.a * (1f - a.a);

                    newLayer.SetPixel(x, y, c);
                }
            }

            newLayer.Apply();

            return newLayer;
        }

        public ITexture HardLight(ITexture baseLayer, ITexture layer) {
            var newLayer = _builder.CreateTexture(baseLayer.width, baseLayer.height);

            for (int x = 0; x < baseLayer.width; x++) {
                for (int y = 0; y < baseLayer.height; y++) {
                    var a = baseLayer.GetPixel(x, y);
                    var b = layer.GetPixel(x, y);

                    var c = new InternalColor();

                    c.r = ColorBlends.HardLight(a.r, b.r);
                    c.g = ColorBlends.HardLight(a.g, b.g);
                    c.b = ColorBlends.HardLight(a.b, b.b);

                    c = ((1f - b.a) * a) + (b.a * c);
                    c.a = a.a + b.a * (1f - a.a);

                    newLayer.SetPixel(x, y, c);
                }
            }

            newLayer.Apply();

            return newLayer;
        }

        public ITexture SoftLight(ITexture baseLayer, ITexture layer) {
            var newLayer = _builder.CreateTexture(baseLayer.width, baseLayer.height);

            for (int x = 0; x < baseLayer.width; x++) {
                for (int y = 0; y < baseLayer.height; y++) {
                    var a = baseLayer.GetPixel(x, y);
                    var b = layer.GetPixel(x, y);

                    var c = new InternalColor();

                    c.r = ColorBlends.SoftLight(a.r, b.r);
                    c.g = ColorBlends.SoftLight(a.g, b.g);
                    c.b = ColorBlends.SoftLight(a.b, b.b);

                    c = ((1f - b.a) * a) + (b.a * c);
                    c.a = a.a + b.a * (1f - a.a);

                    newLayer.SetPixel(x, y, c);
                }
            }

            newLayer.Apply();

            return newLayer;
        }

        public ITexture Difference(ITexture baseLayer, ITexture layer) {
            var newLayer = _builder.CreateTexture(baseLayer.width, baseLayer.height);

            for (int x = 0; x < baseLayer.width; x++) {
                for (int y = 0; y < baseLayer.height; y++) {
                    var a = baseLayer.GetPixel(x, y);
                    var b = layer.GetPixel(x, y);

                    var c = new InternalColor();

                    c.r = ColorBlends.Difference(a.r, b.r);
                    c.g = ColorBlends.Difference(a.g, b.g);
                    c.b = ColorBlends.Difference(a.b, b.b);

                    c = ((1f - b.a) * a) + (b.a * c);
                    c.a = a.a + b.a * (1f - a.a);

                    newLayer.SetPixel(x, y, c);
                }
            }

            newLayer.Apply();

            return newLayer;
        }

        public ITexture Exclusion(ITexture baseLayer, ITexture layer) {
            var newLayer = _builder.CreateTexture(baseLayer.width, baseLayer.height);

            for (int x = 0; x < baseLayer.width; x++) {
                for (int y = 0; y < baseLayer.height; y++) {
                    var a = baseLayer.GetPixel(x, y);
                    var b = layer.GetPixel(x, y);

                    var c = new InternalColor();

                    c.r = ColorBlends.Exclusion(a.r, b.r);
                    c.g = ColorBlends.Exclusion(a.g, b.g);
                    c.b = ColorBlends.Exclusion(a.b, b.b);

                    c = ((1f - b.a) * a) + (b.a * c);
                    c.a = a.a + b.a * (1f - a.a);

                    newLayer.SetPixel(x, y, c);
                }
            }

            newLayer.Apply();

            return newLayer;
        }



        public ITexture Hue(ITexture baseLayer, ITexture layer) {
            var newLayer = _builder.CreateTexture(baseLayer.width, baseLayer.height);

            for (int x = 0; x < baseLayer.width; x++) {
                for (int y = 0; y < baseLayer.height; y++) {
                    var a = baseLayer.GetPixel(x, y);
                    var b = layer.GetPixel(x, y);

                    var s = ColorBlends.Sat(a);
                    var l = ColorBlends.Lum(a);

                    var c = ColorBlends.SetLum(ColorBlends.SetSat(b, s), l);

                    c = ((1f - b.a) * a) + (b.a * c);
                    c.a = a.a + b.a * (1f - a.a);

                    newLayer.SetPixel(x, y, c);
                }
            }

            newLayer.Apply();

            return newLayer;
        }

        public ITexture Saturation(ITexture baseLayer, ITexture layer) {
            var newLayer = _builder.CreateTexture(baseLayer.width, baseLayer.height);

            for (int x = 0; x < baseLayer.width; x++) {
                for (int y = 0; y < baseLayer.height; y++) {
                    var a = baseLayer.GetPixel(x, y);
                    var b = layer.GetPixel(x, y);

                    var s = ColorBlends.Sat(b);
                    var l = ColorBlends.Lum(a);

                    var c = ColorBlends.SetLum(ColorBlends.SetSat(a, s), l);

                    c = ((1f - b.a) * a) + (b.a * c);
                    c.a = a.a + b.a * (1f - a.a);

                    newLayer.SetPixel(x, y, c);
                }
            }

            newLayer.Apply();

            return newLayer;
        }

        public ITexture Color(ITexture baseLayer, ITexture layer) {
            var newLayer = _builder.CreateTexture(baseLayer.width, baseLayer.height);

            for (int x = 0; x < baseLayer.width; x++) {
                for (int y = 0; y < baseLayer.height; y++) {
                    var a = baseLayer.GetPixel(x, y);
                    var b = layer.GetPixel(x, y);

                    var c = ColorBlends.SetLum(b, ColorBlends.Lum(a));

                    c = ((1f - b.a) * a) + (b.a * c);
                    c.a = a.a + b.a * (1f - a.a);

                    newLayer.SetPixel(x, y, c);
                }
            }

            newLayer.Apply();

            return newLayer;
        }

        public ITexture Luminosity(ITexture baseLayer, ITexture layer) {
            var newLayer = _builder.CreateTexture(baseLayer.width, baseLayer.height);

            for (int x = 0; x < baseLayer.width; x++) {
                for (int y = 0; y < baseLayer.height; y++) {
                    var a = baseLayer.GetPixel(x, y);
                    var b = layer.GetPixel(x, y);



                    var c = ColorBlends.SetLum(a, ColorBlends.Lum(b));

                    c = ((1f - b.a) * a) + (b.a * c); ;
                    c.a = a.a + b.a * (1f - a.a);

                    newLayer.SetPixel(x, y, c);
                }
            }

            newLayer.Apply();

            return newLayer;
        }


        public ITexture Addition(ITexture baseLayer, ITexture layer) {
            var newLayer = _builder.CreateTexture(baseLayer.width, baseLayer.height);

            for (int x = 0; x < baseLayer.width; x++) {
                for (int y = 0; y < baseLayer.height; y++) {
                    var a = baseLayer.GetPixel(x, y);
                    var b = layer.GetPixel(x, y);

                    var c = a + b;

                    c = ((1f - b.a) * a) + (b.a * c); ;
                    c.a = a.a + b.a * (1f - a.a);

                    newLayer.SetPixel(x, y, c);
                }
            }

            newLayer.Apply();

            return newLayer;
        }


        public ITexture Subtract(ITexture baseLayer, ITexture layer) {
            var newLayer = _builder.CreateTexture(baseLayer.width, baseLayer.height);

            for (int x = 0; x < baseLayer.width; x++) {
                for (int y = 0; y < baseLayer.height; y++) {
                    var a = baseLayer.GetPixel(x, y);
                    var b = layer.GetPixel(x, y);

                    var c = a - b;

                    c = ((1f - b.a) * a) + (b.a * c); ;
                    c.a = a.a + b.a * (1f - a.a);

                    newLayer.SetPixel(x, y, c);
                }
            }

            newLayer.Apply();

            return newLayer;
        }


        public ITexture Divide(ITexture baseLayer, ITexture layer) {
            var newLayer = _builder.CreateTexture(baseLayer.width, baseLayer.height);

            for (int x = 0; x < baseLayer.width; x++) {
                for (int y = 0; y < baseLayer.height; y++) {
                    var a = baseLayer.GetPixel(x, y);
                    var b = layer.GetPixel(x, y);

                    var c = new InternalColor(
                        ColorBlends.BlendDivide(a.r, b.r),
                        ColorBlends.BlendDivide(a.g, b.g),
                        ColorBlends.BlendDivide(a.b, b.b)
                        );

                    c = ((1f - b.a) * a) + (b.a * c); ;
                    c.a = a.a + b.a * (1f - a.a);

                    newLayer.SetPixel(x, y, c);
                }
            }

            newLayer.Apply();

            return newLayer;
        }
    }
}

