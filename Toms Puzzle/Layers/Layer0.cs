using Toms_Puzzle.Decoders;

namespace Toms_Puzzle.Layers
{
    class Layer0
    {
        // Straight decode
        public static string DecodeLayer0(string payload, IDecoder decoder)
        {
            string ascii = decoder.Decode(payload);

            return ascii;
        }
    }
}
