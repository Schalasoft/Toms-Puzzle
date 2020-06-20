using System;
using System.Linq;
using System.Text;
using Toms_Puzzle.Decoders;

namespace Toms_Puzzle.Layers
{
    class Layer1
    {
        // Flip bits, right-shift, and decode
        public static string DecodeLayer1(string layer, IDecoder decoder)
        {
            StringBuilder payload = new StringBuilder();
            for (int i = 0; i < layer.Length; i++)
            {
                // Get the int value of the char
                byte byteVal = (byte)layer[i];
                int intVal = (int)byteVal;

                // Convert to binary string
                string binary = Convert.ToString(intVal, 2);

                // Convert to bool array
                bool[] bits = binary.Select(b => b == '1').ToArray();

                // Flip the bits
                byte[] flipped = FlipBits(bits);

                // Right shift the bits
                byte[] rightShifted = RightShiftBits(flipped);

                // Add to payload
                string converted = Encoding.ASCII.GetString(rightShifted);//, 0, rightShifted.Length); // CDG I believe this convert is going wrong because flip bits and right shift appear to be working
                payload.Append(converted);
            }

            // Decode payload
            string ascii = decoder.Decode(payload.ToString());

            return ascii;
        }

        // Flip every second bit e.g 110000 becomes 100101
        private static byte[] FlipBits(bool[] bits)
        {
            byte[] result = new byte[bits.Length];

            // Flip every second bit
            for (int i = 0; i < bits.Length; i++)
            {
                if (i != 0 && ((i + 1) % 2) == 0) // Every 2nd bit is flipped
                {
                    bool bit = bits[i];
                    bool flip = bit ^ true;
                    result[i] = Convert.ToByte(flip); // Add bit and use mask of 1/true to flip bit using XOR
                }
                else
                    result[i] = Convert.ToByte(bits[i]); // Add bit
            }

            return result;
        }

        // Right shift bits e.g 100101 becomes 110010
        private static byte[] RightShiftBits(byte[] bits)
        {
            byte[] result = new byte[bits.Length];

            // Start at first bit, make result bit equal to bit left of its index
            for (int i = 0; i < bits.Length; i++)
            {
                int index = i - 1;
                if (index < 0)
                    index = bits.Length - 1; // If grabbing from the left at 0 we grab the last bit (wrap around)

                result[i] = bits[index]; // Grab the bit to the left
            }

            return result;
        }
    }
}
