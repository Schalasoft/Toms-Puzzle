using System;

namespace Toms_Puzzle.Decoders
{
    public interface IDecoder
    {
        Span<byte> Decode(string payload);
    }
}