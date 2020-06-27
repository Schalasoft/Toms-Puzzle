
using Toms_Puzzle.Decoders;

namespace Toms_Puzzle.Layers
{
    public interface ILayer
    {
        public string Decode(string payload, IDecoder decoder);
    }
}
