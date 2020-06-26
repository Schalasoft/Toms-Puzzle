using System.Collections;

namespace Toms_Puzzle.Utilities
{
    // Extension Methods
    public static class ExtensionMethods
    {
        // BitArray extension method for converting to a byte
        // Convert each bit to a byte using tenary conditional 
        // operator and left bit shifting
        internal static byte GetByte(this BitArray array)
        {
            byte b = 0;
            for (int i = 7; i >= 0; i--)
                b = (byte)((b << 1) | (array[i] ? 1 : 0));
            return b;
        }
    }
}
