using System;
using System.Collections.Generic;
using System.Text;

// CDG Needs cleaned up, doing a lot of bad practices to get the decoding working inititally : Make it work, make it right, make it fast
namespace Toms_Puzzle.Decoders
{
    class MyASCII85Decoder : IDecoder
    {
        // Convert ASCII85 to ASCII
        public Span<Byte> Decode(string payload)
        {
            // Base
            List<int> baseValues = Helper.GetBaseValues(payload);
            Console.WriteLine("BASE VALUES:");
            Console.WriteLine(string.Join(",", baseValues.ToArray()) + Environment.NewLine);

            // Value
            List<Int32> bitValues = Helper.GetBitValues(baseValues);
            Console.WriteLine();
            Console.WriteLine("BIT VALUES:");
            Console.WriteLine(string.Join(",", bitValues.ToArray()) + Environment.NewLine);

            // ASCII (from bit patterns)
            //return Helper.GetBitPatterns(bitValues);
            return new Span<byte>();
        }
    }

    // Class containing helper methods for decoding
    static class Helper
    {
        // Invert the characters base 85 representation
        public static List<int> GetBaseValues(string text)
        {
            List<int> baseValues = new List<int>();
            for (int i = 0; i < text.Length; i += 5) // Grab 5 ASCII
            {
                string ASCII = "";

                try
                {
                    ASCII = text.Substring(i, 5);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Pad end with zeros // CDG not sure if correct
                    ASCII = text.Substring(i, text.Length - i);
                    ASCII = ASCII.PadRight(5, '0');
                }

                foreach (char c in ASCII)
                {
                    byte byteVal = (byte)c;
                    int intVal = (int)byteVal;
                    intVal = intVal - 33; // Change base
                    if (intVal < 0) intVal = 85 + intVal; // CDG wrap around if we get minus numbers
                    baseValues.Add(intVal);
                }
            }

            return baseValues;
        }

        // Get the 4 characters from the 5 values
        public static List<Int32> GetBitValues(List<int> values)
        {
            List<Int32> bitValues = new List<Int32>();
            Int32 bitValue = 0;
            int power = 4;

            // Get all sets of bit values
            for (int i = 0; i < values.Count; i++)
            {
                bitValue += GetBitValue(values[i], power--);

                // Add bit value to list and reset for next 5 chars
                if (i != 0 && i % 5 == 0)
                {
                    bitValues.Add(bitValue);
                    bitValue = 0;
                    power = 4;
                }
            }

            return bitValues;
        }

        // Returns a single summed bit value in a set of 5, used with GetBitValues, power is used as each bit must be multipled by an exponent
        // Return 33 if the operation fails (lowest ASCII85 value) // CDG change when fixed
        private static Int32 GetBitValue(Int32 value, int power)
        {
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
                // Max Int32: 2,147,483,647 // CDG Overflowing here
            }
            catch (OverflowException)
            {
                Console.WriteLine("OVERFLOW!");
            }

            return result;
        }

        // Get all the bit patterns from each 32 bit value
        public static string GetBitPatterns(List<Int32> values)
        {
            string result = "";

            foreach (Int32 value in values)
            {
                result += GetBitPattern(value);
            }

            return result;
        }

        // Get the 32 bit pattern from a 32 bit integer
        private static string GetBitPattern(Int32 value)
        {
            string text = "";

            string bitPattern = Convert.ToString(value, 2);
            if (bitPattern.Length < 32) bitPattern = bitPattern.PadLeft(32, '0'); // CDG when to pad right?

            // Bit pattern
            for (int j = 0; j < bitPattern.Length; j = j + 8)
            {
                string firstVal = bitPattern.Substring(j, 8);
                byte[] b = GetBytesFromBinaryString(firstVal);
                text += ByteToASCII(b);
            }

            return text;
        }

        // Get byte representation from binary string
        public static Byte[] GetBytesFromBinaryString(String binary)
        {
            var list = new List<Byte>();

            for (int i = 0; i < binary.Length; i += 8)
            {
                String t = binary.Substring(i, 8);

                list.Add(Convert.ToByte(t, 2));
            }

            return list.ToArray();
        }

        // Get ASCII character from byte
        public static string ByteToASCII(byte[] data)
        {
            return Encoding.ASCII.GetString(data);
        }
    }
}
