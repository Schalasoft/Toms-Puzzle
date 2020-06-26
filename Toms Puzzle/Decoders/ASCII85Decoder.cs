using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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

            return ascii;
        }

        // Convert ASCII85 to ASCII
        private static Span<byte> DecodeASCII85ToASCII(string text)
        {
            List<byte> bytes = new List<byte>();

            // Any padding we had to add to the last tuple
            int padding = 0; 

            // Grab 5 ASCII85 characters at a time
            for (int i = 0; i < text.Length; i += 5)
            {
                string ascii85 = "";

                // If we aren't at the end of the text
                if(i + 5 < text.Length)
                {
                    // Get 5 characters
                    ascii85 = text.Substring(i, 5);
                }
                else // Special case for an incomplete end 5-tuple (pad with zero bytes)
                {
                    // Grab the incomplete tuple   
                    ascii85 = text.Substring(i, text.Length - i);

                    // Pad end value with max value as we ignore them
                    ascii85 = ascii85.PadRight(5, 'u');

                    // Set the padding amount we used
                    padding = 5 - (text.Length - i);
                }

                // Get the 5 base values from the text
                Int32[] baseValues = new Int32[5];
                for (int j = 0; j < 5; j++)
                {
                    // Change Base
                    baseValues[j] = GetBaseValue(ascii85[j]);
                }

                // Get 32-bit Value
                UInt32 bitValue = Get32BitValue(baseValues);

                // Add the bytes from the 32-bit value to the return list (reversed due to big endian)
                bytes.AddRange(BitConverter.GetBytes(bitValue).Reverse());
            }

            // Return bytes but removing any padding bytes due to an incomplete tuple
            return new Span<byte>(bytes.Take(bytes.Count - padding).ToArray());
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
        private static UInt32 Get32BitValue(Int32[] values)
        {
            // Get 32-bit value by summing and powering each of the 5 characters
            UInt32 bitValue = 0;
            int power = 4;
            for (int i = 0; i < values.Length; i++)
            {
                bitValue += Power(values[i], power--);
            }

            return bitValue;
        }

        // Returns a single summed bit value in a set of 5
        private static UInt32 Power(Int32 value, int power)
        {
            // Result is value multiplied by 85 to an exponent (in the order: 4,3,2,1,0)
            UInt32 result = 0;
            double left = value;
            double right = 0;
            if (power == 0)
                right = 1; // Final value is not multiplied by 85 so just set right to 1 to leave left unchanged
            else
                right = Math.Pow(85, power);

            // Multiply value by 85 to the power of N
            double sum = left * right;

            // Convert result to an Unsigned Int32 to handle overflows in Int32
            result = Convert.ToUInt32(sum);

            return result;
        }
    }
}
