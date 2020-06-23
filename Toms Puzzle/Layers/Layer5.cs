using System;
using Toms_Puzzle.Decoders;

namespace Toms_Puzzle.Layers
{
    class Layer5
    {
        public static string DecodeLayer5(string payload, IDecoder decoder)
        {
            // Decode
            Span<byte> bytes = decoder.Decode(payload);

            // Convert to string
            string result = System.Text.Encoding.Default.GetString(bytes);

            return result;
        }
    }
}
