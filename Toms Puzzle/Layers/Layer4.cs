using System;
using System.Text;
using Toms_Puzzle.Decoders;

namespace Toms_Puzzle.Layers
{
    class Layer4
    {
        public static string DecodeLayer4(string payload, IDecoder decoder)
        {
            // Decode the payload
            byte[] output = decoder.Decode(payload).ToArray();

            // Convert to string
            string result = Encoding.ASCII.GetString(output, 0, output.Length);

            return result;
        }
    }
}
