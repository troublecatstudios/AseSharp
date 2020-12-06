using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AsefileSharp {
    public enum MetaDataType { UNKNOWN, TRANSFORM };

    public class MetaData {
        static public string MetaDataChar = "@";

        public MetaDataType Type { get; private set; }
        //Average position per frames
        public Dictionary<int, (float x, float y)> Transforms { get; private set; }
        public List<string> Args { get; private set; }

        public MetaData(string layerName) {
            var regex = new Regex("@transform\\(\"(.*)\"\\)");
            var match = regex.Match(layerName);
            if (match.Success) {
                Type = MetaDataType.TRANSFORM;
                Args = new List<string>();
                Args.Add(match.Groups[1].Value);
                Transforms = new Dictionary<int, (float x, float y)>();
            } else
                throw new Exception($"Unsupported aseprite metadata {layerName}");
        }
    }
}
