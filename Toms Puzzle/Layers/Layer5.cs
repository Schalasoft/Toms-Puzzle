/*
    To protect the innocent Layer 5 is closed for today, but if you come back and perform a simple edit to one line of code in this file then functionality will be restored
*/

using System;
using System.IO;
using Toms_Puzzle.Interfaces;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using static Toms_Puzzle.Utilities.ByteConverter;

namespace Toms_Puzzle.Layers
{
    class Layer5 : ILayer
    {
        // Properties
        public static byte[] KeyEncryptingKey { get; private set; }
        public static byte[] KeyInitializationVector { get; private set; }
        public static byte[] WrappedKey { get; private set; }
        public static byte[] PayloadEncryptionKey { get; private set; } // The unwrapped key
        public static byte[] PayloadInitializationVector { get; private set; }

        // Decode the layer
        public string Decode(string payload, IDecoder decoder)
        {
            // Decode
            Span<byte> bytes = decoder.Decode(payload);

            // Add bytes to a memory stream
            MemoryStream stream = new MemoryStream(bytes.ToArray());

            // Get keys and IVs
            GetAES(stream);

            // Get the encrypted payload
            byte[] encryptedPayload = GetEncryptedPayload(stream);

            // Unwrap the wrapped key
            byte[] decryptedPayloadBytes = Decrypt(encryptedPayload);

            // Decode the payload
            string decryptedPayload = System.Text.Encoding.Default.GetString(decryptedPayloadBytes, 0, decryptedPayloadBytes.Length - 5); // Ignore last 5 bytes, they're junk

            return decryptedPayload;
        }

        // Unwraps a wrapped key using a key encrypting key (KEK) and Initialization Vector (IV)
        private byte[] UnwrapKey()
        {
            // Create Bouncy Castle AES Wrap Engine
            IWrapper wrapper = new AesWrapEngine();

            // Initialize engine with the KEK and IV
            wrapper.Init(false, new ParametersWithIV(new KeyParameter(KeyEncryptingKey), KeyInitializationVector));
            
            // Unwrap the wrapped key
            byte[] unwrapped = wrapper.Unwrap(WrappedKey, 0, WrappedKey.Length);

            return unwrapped;
        }

        // Decrypt the encrypted payload
        private byte[] Decrypt(byte[] encryptedData)
        {
            // Unwrap the KEK
            PayloadEncryptionKey = UnwrapKey();

            // Setup the decryption cipher
            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/ECB/NoPadding");
            cipher.Init(false, new ParametersWithIV(new KeyParameter(PayloadEncryptionKey), PayloadInitializationVector));

            // Decrypt
            byte[] decrypted = cipher.ProcessBytes(encryptedData);

            return decrypted;
        }

        // Gets the AES keys and initialization vectors
        private void GetAES(MemoryStream stream)
        {
            // Get 256-bit Key Encrypting Key (32 bytes)
            int kekSize = 32;
            KeyEncryptingKey = GetBytes(stream, kekSize);

            // Get 64-bit Initialization Vector (8 bytes)
            int ivSize = 8;
            KeyInitializationVector = GetBytes(stream, ivSize);

            // Get the wrapped (encrypted) key (40 bytes)
            // When unwrapped this will become the 256-bit Encryption Key
            int ekSize = 40;
            WrappedKey = GetBytes(stream, ekSize);

            // Get the 128-bit Initialization Vector for the encrypted payload (16 bytes)
            int eivSize = 16;
            PayloadInitializationVector = GetBytes(stream, eivSize);
        }

        // Get the encrypted payload from memory stream
        private byte[] GetEncryptedPayload(MemoryStream stream)
        {
            // Create byte array the same size as the data remaining in the stream
            byte[] payload = new byte[stream.Length - stream.Position];

            // Read the stream into the byte array
            stream.Read(payload);

            return payload;
        }
    }
}
