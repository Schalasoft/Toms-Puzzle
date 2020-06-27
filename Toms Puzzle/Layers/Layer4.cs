using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Toms_Puzzle.Interfaces;
using Toms_Puzzle.Utilities;

namespace Toms_Puzzle.Layers
{
    // Decode class
    class Layer4 : ILayer
    {
        // Decode the layer
        public string Decode(string payload, IDecoder decoder)
        {
            // Decode
            Span<byte> decodedBytes = decoder.Decode(payload);

            // Get the payloads from the packets
            List<byte[]> packetPayloads = ExtractPayloads(decodedBytes.ToArray());

            // Combine the payloads
            List<byte> combinedPayload = new List<byte>();
            foreach(byte[] packetPayload in packetPayloads)
            {
                // Add payload to combined payload
                combinedPayload.AddRange(packetPayload);
            }
            
            // Convert to string
            string result = Encoding.ASCII.GetString(combinedPayload.ToArray(), 0, combinedPayload.Count);

            return result;
        }

        // Extract and return all valid payloads from the bytes in a list
        private List<byte[]> ExtractPayloads(byte[] bytes)
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
