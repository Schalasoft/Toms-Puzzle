using System;
using System.IO;
using Toms_Puzzle.Decoders;
using static Toms_Puzzle.Utilities.ByteConverter;

namespace Toms_Puzzle.Layers
{
    class Layer5
    {
        public static string DecodeLayer5(string payload, IDecoder decoder)
        {
            // Decode
            Span<byte> bytes = decoder.Decode(payload);

            // Add bytes to a memory stream
            MemoryStream stream = new MemoryStream(bytes.ToArray());

            // Get keys and IVs
            GetAES(stream);

            // Get the encrypted payload
            byte[] encryptedPayload = GetEncryptedPayload(stream);

            // Convert to string
            string result = System.Text.Encoding.Default.GetString(encryptedPayload);

            return result;
        }

        // Gets the AES keys and initialization vectors
        private static void GetAES(MemoryStream stream)
        {
            // Get 256-bit Key Encrypting Key (32 bytes)
            int kekSize = 32;
            byte[] kek = GetBytes(stream, kekSize);

            // Get 64-bit Initialization Vector (8 bytes)
            int ivSize = 8;
            byte[] iv = GetBytes(stream, ivSize);

            // Get the wrapped (encrypted) key (40 bytes)
            // When unwrapped this will become the 256-bit Encryption Key
            int ekSize = 40;
            byte[] ek = GetBytes(stream, ekSize);

            // Get the 128-bit Initialization Vector for the encrypted payload (16 bytes)
            int eivSize = 16;
            byte[] eiv = GetBytes(stream, eivSize);
        }

        // Get the encrypted payload from memory stream
        private static byte[] GetEncryptedPayload(MemoryStream stream)
        {
            // Create byte array the same size as the data remaining in the stream
            byte[] payload = new byte[stream.Length - stream.Position];

            // Read the stream into the byte array
            stream.Read(payload);

            return payload;
        }
    }
}
