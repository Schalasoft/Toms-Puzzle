using System;
using Toms_Puzzle.Decoders;

namespace Toms_Puzzle.Layers
{
    class Layer2
    {
        public static string DecodeLayer2(string payload, IDecoder decoder)
        {
            Span<byte> bytes = decoder.Decode(payload);

            string result = "";

            return result;
        }
    }
}
