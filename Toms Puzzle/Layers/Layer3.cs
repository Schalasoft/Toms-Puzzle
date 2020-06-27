using System;
using System.Collections;
using System.Linq;
using System.Text;
using Toms_Puzzle.Decoders;
using static Toms_Puzzle.Utilities.ExtensionMethods;

namespace Toms_Puzzle.Layers
{
    class Layer3 : ILayer
    {
        // Decode layer 3
        public string Decode(string payload, IDecoder decoder)
        {
            // Decode the payload
            Span<byte> bytes = decoder.Decode(payload);

            // Decrypt the payload
            byte[] output = Decrypt(bytes.ToArray());

            // Convert to string
            string result = Encoding.ASCII.GetString(output, 0, output.Length);

            return result;
        }

        // Decrypt payload using a 32 byte cycled key
        private byte[] Decrypt(byte[] bytes)
        {
            // Determine the cycled key
            byte[] cycledKey = DetermineCycledKey(bytes);

            // Decrypted result
            byte[] result = new byte[bytes.Length];

            // Run the cycled key against the payload to decrypt it
            for (int i = 0; i < bytes.Length; i++)
            {
                // Decrypt
                result[i] = xorDecrypt(bytes[i], cycledKey[i % cycledKey.Length]);
            }

            return result;
        }

        // Determine the actual key by getting a partial decryption
        // Then xoring the encrypted text (C)
        // with the decrypted text        (A)
        // to get the encryption key      (B)
        private byte[] DetermineCycledKey(byte[] bytes)
        {
            // Data we expect to be in the payload (32 chars)
            const string expectedSequence = "--------------------------------";

            // XOR the first 32 bytes with the expected 32 byte sequence
            byte[] expectedKey = DetermineCycledKeyUsingExpected(bytes, expectedSequence);
            byte[] expectedBytes = new byte[bytes.Length];
            for(int i = 0; i < bytes.Length; i++)
            {
                expectedBytes[i] = xorDecrypt(bytes[i], expectedKey[i % expectedKey.Length]);
            }

            // Get the plain text of that XOR
            string partialDecryptionText = Encoding.ASCII.GetString(expectedBytes, 0, expectedBytes.Length);

            // Data we know is in the partial decryption from the payload
            const string knownSequence = "==[ Layer ";

            // Get the initial index
            int index = partialDecryptionText.IndexOf(knownSequence);

            // Grab 32 bytes from the index
            string decryptedSequence = partialDecryptionText.Substring(index, 32);

            // Get the actual key by doing an XOR on the first 32 bytes using the decrypted sequence 
            // key == encrypted xor unencrypted
            byte[] key = DetermineCycledKeyUsingExpected(bytes, decryptedSequence);

            return key;
        }

        // Determine a cycled key using expected data
        private byte[] DetermineCycledKeyUsingExpected(byte[] bytes, string text)
        {
            // Get the bytes for the known text
            byte[] textBytes = Encoding.ASCII.GetBytes(text);

            // Convert to bit array
            BitArray textBitArray = new BitArray(textBytes);

            // Get an equal number of bytes from the start of the payload
            byte[] payloadBytes = bytes.Take(textBytes.Length).ToArray();
            BitArray payloadBitArray = new BitArray(payloadBytes);

            // Decrypt
            BitArray keyBitArray = textBitArray.Xor(payloadBitArray);

            // Convert
            byte[] key = new byte[32];
            keyBitArray.CopyTo(key, 0);

            return key;
        }

        // Takes in A and B and returns C from an XOR operation
        private byte xorDecrypt(byte a, byte b)
        {
            // Perform XOR
            BitArray partA = new BitArray(new byte[1] { a });
            BitArray partB = new BitArray(new byte[1] { b });
            BitArray partC = partA.Xor(partB);

            // Convert to byte
            return partC.GetByte();
        }
    }
}
