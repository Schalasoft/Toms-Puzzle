using System;
using System.Text;
using Toms_Puzzle.Interfaces;

namespace Toms_Puzzle.Layers
{
    class Layer0 : ILayer
    {
        public string Decode(string payload, IDecoder decoder)
        {
            // Decode
            Span<byte> bytes = decoder.Decode(payload);

            // Convert to string
            string result = Encoding.ASCII.GetString(bytes);

            return result;
        }
    }
}
