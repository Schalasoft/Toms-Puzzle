using System;
using Toms_Puzzle.Decoders;

namespace Toms_Puzzle.Layers
{
    class Layer0
    {
        // Straight decode
        public static string DecodeLayer0(string payload, IDecoder decoder)
        {
            Span<byte> bytes = decoder.Decode(payload);

            string result = System.Text.Encoding.Default.GetString(bytes);

            return result;
        }
    }
}
