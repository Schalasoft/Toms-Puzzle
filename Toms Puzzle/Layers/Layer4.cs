using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Toms_Puzzle.Decoders;
using Toms_Puzzle.Utilities;

namespace Toms_Puzzle.Layers
{
    // Decode class
    class Layer4
    {
        public static string DecodeLayer4(string payload, IDecoder decoder)
        {
            // Decode
            Span<byte> decodedBytes = decoder.Decode(payload);
            byte[] bytes = decodedBytes.ToArray();

            // Get the payloads from the packets
            List<byte[]> payloads = ExtractPayloads(bytes);

            // Determine the size of the combined payload array // CDG I dont like this method
            int combinedPayloadLength = 0;
            foreach (byte[] packetPayload in payloads)
                combinedPayloadLength += packetPayload.Length;

            // Combine the payloads
            byte[] combinedPayload = new byte[combinedPayloadLength];
            int currentIndex = 0;
            for(int i = 0; i < payloads.Count; i++)
            {
                // Copy in packet payload to combine payload at the correst index
                payloads[i].CopyTo(combinedPayload, currentIndex);

                // Increment the index by the length of the packet payload
                currentIndex = payloads[i].Length;
            }
            
            // Convert to string
            string result = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

            return result;
        }

        // Extract and return all valid payloads from the bytes in a list
        private static List<byte[]> ExtractPayloads(byte[] bytes)
        {
            // Convert to a memory stream for convenience
            MemoryStream stream = new MemoryStream(bytes);

            // List of byte arrays to return
            List<byte[]> payloads = new List<byte[]>();

            // Parse the bytes
            for(int i = 0; i < bytes.Length; i++)
            {
                // Assemble packet
                Packet packet = new Packet(stream);

                // If the packet is valid then add the packets data to the combined list
                if (packet.Valid)
                    payloads.Add(packet.Data);
            }

            return payloads;
        }
    }
}
