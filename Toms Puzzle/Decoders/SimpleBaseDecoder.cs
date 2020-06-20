using System;

namespace Toms_Puzzle.Decoders
{
    class SimpleBaseDecoder : IDecoder
    {
        // Decode ASCII85 to ASCII
        public string Decode(string payload)
        {
            Span<byte> bytes = SimpleBase.Base85.Ascii85.Decode(payload);
            string ascii = System.Text.Encoding.Default.GetString(bytes);

            return ascii;
        }
    }
}
