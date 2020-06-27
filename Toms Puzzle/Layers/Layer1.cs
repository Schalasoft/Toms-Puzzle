using System.Collections;
using System.Text;
using Toms_Puzzle.Decoders;

namespace Toms_Puzzle.Layers
{
    class Layer1 : ILayer
    {
        // Decode, then Flip bits, followed by a right-shift
        public string Decode(string payload, IDecoder decoder)
        {
            // Decode the payload
            byte[] bytes = decoder.Decode(payload).ToArray();

            // Perform bit manipulations
            byte[] output = FlipRightShift(bytes);

            // Convert to string to get the actual payload
            string result = Encoding.ASCII.GetString(output);

            return result;
        }

        private byte[] FlipRightShift(byte[] bytes)
        {
            // Create a bitmask to only flip every second bit (read from right to left, true means flip that bit)
            BitArray flipMask = new BitArray(new bool[8] { true, false, true, false, true, false, true, false });

            // Manipulate the bytes
            byte[] result = new byte[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                // Put byte into a size 1 bit array
                BitArray bitArray = new BitArray(new byte[1] { bytes[i] });

                // Flip every second bit using flip mask
                bitArray = bitArray.Xor(flipMask);

                // Take a copy of the last bit as this will be lost on a right shift
                bool lastBit = bitArray[0];

                // Right shift the array by 1 position
                bitArray = bitArray.RightShift(1);

                // Put the last bit in the first position (wrapping around)
                bitArray[bitArray.Length - 1] = lastBit;

                // Copy flipped and shifted array to the output array
                bitArray.CopyTo(result, i);
            }

            return result;
        }
    }
}
