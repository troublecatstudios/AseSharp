﻿using AsefileSharp.Abstractions;
using System;
using System.Collections.Generic;

namespace AsefileSharp.Utils {
    public static class ColorBlends {
        public static float Multiply(float b, float s) {
            return b * s;
        }

        public static float Screen(float b, float s) {
            return b + s - (b * s);
        }

        public static float Overlay(float b, float s) {
            return HardLight(s, b);
        }

        public static float Darken(float b, float s) {
            return InternalMath.Min(b, s);
        }

        public static float Lighten(float b, float s) {
            return InternalMath.Max(b, s);
        }

        // Color Dodge & Color Burn:  http://wwwimages.adobe.com/www.adobe.com/content/dam/Adobe/en/devnet/pdf/pdfs/adobe_supplement_iso32000_1.pdf
        public static float ColorDodge(float b, float s) {
            if (b == 0)
                return 0;
            else if (b >= (1 - s))
                return 1;
            else
                return b / (1 - s);
        }

        public static float ColorBurn(float b, float s) {
            if (b == 1)
                return 1;
            else if ((1 - b) >= s)
                return 0;
            else
                return 1 - ((1 - b) / s);
        }

        public static float HardLight(float b, float s) {
            if (s <= 0.5)
                return Multiply(b, 2 * s);
            else
                return Screen(b, 2 * s - 1);
        }

        public static float SoftLight(float b, float s) {
            if (s <= 0.5)
                return b - (1 - 2 * s) * b * (1 - b);
            else
                return b + (2 * s - 1) * (SoftLightD(b) - b);
        }

        internal static float SoftLightD(float x) {
            if (x <= 0.25)
                return ((16 * x - 12) * x + 4) * x;
            else
                return (float)Math.Sqrt(x);
        }

        public static float Difference(float b, float s) {
            return Math.Abs(b - s);
        }

        public static float Exclusion(float b, float s) {
            return b + s - 2 * b * s;
        }

        internal static float BlendDivide(float b, float s) {
            if (b == 0)
                return 0;
            else if (b >= s)
                return 255;
            else
                return b / s;
        }


        internal static double Lum(IColor c) {
            return (0.3 * c.r) + (0.59 * c.g) + (0.11 * c.b);
        }

        internal static InternalColor ClipColor(IColor c) {
            double l = Lum(c);
            float n = Math.Min(c.r, Math.Min(c.g, c.b));
            float x = Math.Max(c.r, Math.Max(c.g, c.b));


            if (n < 0) {
                c.r = (float)(l + (((c.r - l) * l) / (l - n)));
                c.g = (float)(l + (((c.g - l) * l) / (l - n)));
                c.b = (float)(l + (((c.b - l) * l) / (l - n)));
            }
            if (x > 1) {
                c.r = (float)(l + (((c.r - l) * (1 - l)) / (x - l)));
                c.g = (float)(l + (((c.g - l) * (1 - l)) / (x - l)));
                c.b = (float)(l + (((c.b - l) * (1 - l)) / (x - l)));
            }

            return new InternalColor(c);
        }




        internal static InternalColor SetLum(IColor c, double l) {
            double d = l - Lum(c);
            c.r = (float)(c.r + d);
            c.g = (float)(c.g + d);
            c.b = (float)(c.b + d);

            return ClipColor(c);
        }

        internal static double Sat(IColor c) {
            return Math.Max(c.r, Math.Max(c.g, c.b)) - Math.Min(c.r, Math.Min(c.g, c.b));
        }

        internal static double DMax(double x, double y) { return (x > y) ? x : y; }
        internal static double DMin(double x, double y) { return (x < y) ? x : y; }




        internal static InternalColor SetSat(IColor c, double s) {
            char cMin = GetMinComponent(c);
            char cMid = GetMidComponent(c);
            char cMax = GetMaxComponent(c);

            double min = GetComponent(c, cMin);
            double mid = GetComponent(c, cMid);
            double max = GetComponent(c, cMax);


            if (max > min) {
                mid = ((mid - min) * s) / (max - min);
                c = SetComponent(c, cMid, (float)mid);
                max = s;
                c = SetComponent(c, cMax, (float)max);
            } else {
                mid = max = 0;
                c = SetComponent(c, cMax, (float)max);
                c = SetComponent(c, cMid, (float)mid);
            }

            min = 0;
            c = SetComponent(c, cMin, (float)min);

            return new InternalColor(c);
        }




        internal static float GetComponent(IColor c, char component) {
            switch (component) {
                case 'r': return c.r;
                case 'g': return c.g;
                case 'b': return c.b;
            }

            return 0f;
        }


        internal static InternalColor SetComponent(IColor c, char component, float value) {
            switch (component) {
                case 'r': c.r = value; break;
                case 'g': c.g = value; break;
                case 'b': c.b = value; break;
            }

            return new InternalColor(c);
        }

        internal static char GetMinComponent(IColor c) {
            var r = new KeyValuePair<char, float>('r', c.r);
            var g = new KeyValuePair<char, float>('g', c.g);
            var b = new KeyValuePair<char, float>('b', c.b);

            return MIN(r, MIN(g, b)).Key;
        }

        internal static char GetMidComponent(IColor c) {
            var r = new KeyValuePair<char, float>('r', c.r);
            var g = new KeyValuePair<char, float>('g', c.g);
            var b = new KeyValuePair<char, float>('b', c.b);

            return MID(r, g, b).Key;
        }

        internal static char GetMaxComponent(IColor c) {
            var r = new KeyValuePair<char, float>('r', c.r);
            var g = new KeyValuePair<char, float>('g', c.g);
            var b = new KeyValuePair<char, float>('b', c.b);

            return MAX(r, MAX(g, b)).Key;
        }

        internal static KeyValuePair<char, float> MIN(KeyValuePair<char, float> x, KeyValuePair<char, float> y) {
            return (x.Value < y.Value) ? x : y;
        }

        internal static KeyValuePair<char, float> MAX(KeyValuePair<char, float> x, KeyValuePair<char, float> y) {
            return (x.Value > y.Value) ? x : y;
        }

        internal static KeyValuePair<char, float> MID(KeyValuePair<char, float> x, KeyValuePair<char, float> y, KeyValuePair<char, float> z) {
            List<KeyValuePair<char, float>> components = new List<KeyValuePair<char, float>>();
            components.Add(x);
            components.Add(z);
            components.Add(y);


            components.Sort((c1, c2) => { return c1.Value.CompareTo(c2.Value); });

            return components[1];
            //return MAX(x, MIN(y, z));
        }
    }
}
