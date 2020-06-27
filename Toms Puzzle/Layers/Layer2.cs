using System;
using System.Collections;
using System.Text;
using Toms_Puzzle.Decoders;

namespace Toms_Puzzle.Layers
{
    class Layer2 : ILayer
    {
        public string Decode(string payload, IDecoder decoder)
        {
            // Decode the payload
            Span<byte> bytes = decoder.Decode(payload);

            // Construct output from validated data in the payload
            byte[] output = ValidateData(bytes.ToArray());

            // Convert to string
            string result = Encoding.ASCII.GetString(output, 0, output.Length);

            return result;
        }

        // Parse the data and construct the valid dataset from valid bytes
        private byte[] ValidateData(byte[] bytes)
        {
            BitArray bits = new BitArray(bytes.Length * 7); // 7 data bits to combine from original so the length is 7 times larger

            // Track byte section we're at
            int bitCount = 0;
            int bitIndex = 7;

            // Check each byte
            for (int i = 0; i < bytes.Length; i++)
            {
                // Check each bit in a byte
                BitArray bitArray = new BitArray(new byte[1] { bytes[i] });

                // Proceed if the parity check passes
                if (CheckParity(bitArray))
                {
                    // Copy 7 bits to bit array we are building
                    for (int j = 7; j > 0; j--)
                    {
                        // Set the bit value in the correct position for the output
                        bits.Set(bitIndex + bitCount, bitArray[j]);

                        // Move to next bit
                        bitIndex--;

                        // When we get to the parity bit we move to the next byte
                        if (bitIndex < 0)
                        {
                            bitIndex = 7;
                            bitCount += 8;
                        }
                    }
                }
            }      

            // Copy bits into byte array
            byte[] output = new byte[bits.Length];
            bits.CopyTo(output, 0);

            return output;
        }

        // If the sum of the 7 most significant bits are...
        // Even AND Parity bit is 0 : return true
        // Odd  AND Parity bit is 1 : return true
        private bool CheckParity(BitArray bitArray)
        {
            // Default to false
            bool result = false;

            // Count 1's in significant bits
            int bitCount = 0;
            for(int i = 7; i > 0; i--)
            {
                if (bitArray[i].Equals(true)) bitCount++;
            }

            // Return true if valid
            if ((bitCount % 2 == 0) == true && bitArray[0] == false
                ||
                (bitCount % 2 != 0) == true && bitArray[0] == true)
                result = true;

            return result;
        }
    }
}
