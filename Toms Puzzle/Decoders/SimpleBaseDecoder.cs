using System;
using Toms_Puzzle.Interfaces;

namespace Toms_Puzzle.Decoders
{
    class SimpleBaseDecoder : IDecoder
    {
        // Decode ASCII85 to ASCII
        public Span<byte> Decode(string payload)
        {
            Span<byte> bytes = SimpleBase.Base85.Ascii85.Decode(payload);

            return bytes;
        }
    }
}
