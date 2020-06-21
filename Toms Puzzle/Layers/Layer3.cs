using System;
using System.Text;
using Toms_Puzzle.Decoders;

namespace Toms_Puzzle.Layers
{
    class Layer3
    {
        public static string DecodeLayer3(string payload, IDecoder decoder)
        {
            // Decode the payload
            Span<byte> bytes = decoder.Decode(payload);

            // Decrypt the payload
            byte[] output = Decrypt(bytes.ToArray());

            // Convert to string
            string result = Encoding.ASCII.GetString(output, 0, output.Length);

            return result;
        }

        private static byte[] Decrypt(byte[] bytes)
        {
            /*
             Aruba Jamaica ooo I wanna take you
             Bermuda Bahama come on pretty mama
             Key Largo Montego baby why don't we go
             Jamaica
            */

            byte[] result = new byte[8];

            return result;
        }
    }
}
