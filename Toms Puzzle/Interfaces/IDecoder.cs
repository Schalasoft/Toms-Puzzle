using System;

namespace Toms_Puzzle.Interfaces
{
    public interface IDecoder
    {
        Span<byte> Decode(string payload);
    }
}