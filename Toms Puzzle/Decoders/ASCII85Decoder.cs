using System;
using System.Collections.Generic;
using System.Linq;

// CDG Needs cleaned up, doing a lot of bad practices to get the decoding working inititally : Make it work, make it right, make it fast
namespace Toms_Puzzle.Decoders
{
    class ASCII85Decoder : IDecoder
    {
        // Convert ASCII85 string to ASCII byte span
        public Span<byte> Decode(string payload)
        {
            // Get the ascii bytes from the ASCII85 payload
            Span<byte> ascii = DecodeASCII85ToASCII(payload);

            // Return bytes
            byte[] bytes = new byte[ascii.Length];
            Buffer.BlockCopy(ascii.ToArray(), 0, bytes, 0, bytes.Length);

            return new Span<byte>(bytes);
        }

        // Convert ASCII85 to ASCII
        private static Span<byte> DecodeASCII85ToASCII(string text)
        {
            List<byte> bytes = new List<byte>();

            // Grab 5 ASCII85 characters at a time
            for (int i = 0; i < text.Length; i += 5)
            {
                string ascii85 = "";

                // If we aren't as the end of the text
                if(i + 5 < text.Length)
                {
                    // Get 5 characters
                    ascii85 = text.Substring(i, 5);
                }
                else // Special case for an incomplete end 5-tuple (pad with zero bytes) // CDG not sure if its always missing only 3 values
                {
                    // Pad end value with zero bytes as we ignore them
                    ascii85 = text.Substring(i, text.Length - i);
                    ascii85 += "YkO"; // These values will zero out the first 3 bytes in the bit value
                }

                // Get the 5 base values from the text
                Int32[] baseValues = new Int32[5];
                for (int j = 0; j < 5; j++)
                {
                    // Change Base
                    baseValues[j] = GetBaseValue(ascii85[j]);
                }

                // Get 32-bit Value
                Int32 bitValue = Get32BitValue(baseValues);

                // Add the bytes from the 32-bit value to the return list (reversed due to big endian)
                bytes.AddRange(BitConverter.GetBytes(bitValue).Reverse());
            }

            return new Span<byte>(bytes.ToArray());
        }

        // Invert the characters base 85 representation
        private static Int32 GetBaseValue(char ascii85)
        {
            // Get the byte
            byte byteVal = (byte)ascii85;

            // Convert to Int32
            Int32 baseValue = (Int32)byteVal;

            // Change base
            baseValue = baseValue - 33;

            return baseValue;
        }

        // Get the 32-bit value from the rebased characters
        private static Int32 Get32BitValue(Int32[] values)
        {
            // Get 32-bit value by summing and powering each of the 5 characters
            Int32 bitValue = 0;
            int power = 4;
            for (int i = 0; i < values.Length; i++)
            {
                bitValue += Power(values[i], power--);
            }

            return bitValue;
        }

        // Returns a single summed bit value in a set of 5, used with GetBitValues, power is used as each bit must be multipled by an exponent
        private static Int32 Power(Int32 value, int power)
        {
            // Default result to 33 as it is the smallest valid value to return
            Int32 result = 33;
            try
            {
                double left = value;
                double right = 0;
                if (power == 0)
                    right = 1;
                else
                    right = Math.Pow(85, power);

                double sum = left * right;
                result = Convert.ToInt32(sum);
            }
            catch (OverflowException)
            {
                Console.WriteLine($"Value has caused an overflow: {0}", value);
            }

            return result;
        }
    }
}
