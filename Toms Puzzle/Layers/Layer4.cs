using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Toms_Puzzle.Decoders;

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

            //byte[] bytes = Encoding.ASCII.GetBytes(payload);

            // Get the payloads from the packets
            
            List<byte[]> packetPayloads = ExtractPayloads(bytes);

            // Determine the size of the combined payload array
            int combinedPayloadLength = 0;
            foreach (byte[] packetPayload in packetPayloads)
                combinedPayloadLength += packetPayload.Length;

            // Combine the payloads
            byte[] combinedPayload = new byte[combinedPayloadLength];
            int currentIndex = 0;
            for(int i = 0; i < packetPayloads.Count; i++)
            {
                // Copy in packet payload to combine payload at the correst index
                packetPayloads[i].CopyTo(combinedPayload, currentIndex);

                // Increment the index by the length of the packet payload
                currentIndex = packetPayloads[i].Length;
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
            List<byte[]> combinedPayloads = new List<byte[]>();

            // Parse the bytes
            for(int i = 0; i < bytes.Length; i++)
            {
                // Assemble packet
                Packet packet = new Packet(stream);

                // If the packet is valid then add the packets data to the combined list
                if (packet.Valid)
                    combinedPayloads.Add(packet.Data);
            }

            return new List<byte[]> { bytes };
        }
    }

    // IPv4 Packet class
    class Packet
    {
        // Fixed attributes of a packet
        private const int IPHeaderLength = 20;
        private const int UDPHeaderLength = 8;
        private const int HeadersLength = IPHeaderLength + UDPHeaderLength;

        // Validation
        private const string ValidFromAddress = "10.1.1.10";
        private const string ValidToAddress = "10.1.1.200";
        private const int ValidToPort = 42069;

        // Properties
        public string SourceIP { get; private set; }
        public string DestinationIP { get; private set; }
        public int DestinationPort { get; private set; }
        public int TotalLength { get; private set; }
        public int IPv4HeaderChecksum { get; private set; }
        public bool IPv4HeaderChecksumValid { get; private set; }
        public int UDPHeaderChecksum { get; private set; }
        public bool UDPHeaderChecksumValid { get; private set; }
        public byte[] Data { get; private set; }
        public bool Valid { get; private set; }

        // Constructor
        public Packet(MemoryStream stream)
        {
            // Consume the packet
            ConsumePacket(stream);

            // Validate the packet
            ValidatePacket();
        }

        // Consumes the packet to set the class properties
        private void ConsumePacket(MemoryStream stream)
        {
            // Get IPv4 Header properties (20 bytes)
            GetBytes(stream, 2); // Consume bytes we don't care about
            this.TotalLength = GetUInt16(stream); // Consumes 2 bytes
            GetBytes(stream, 6);
            this.IPv4HeaderChecksum = GetUInt16(stream);
            this.SourceIP = GetIP(GetBytes(stream, 4));
            this.DestinationIP = GetIP(GetBytes(stream, 4));

            // Get UDP Header properties (8 bytes)
            GetBytes(stream, 2); // Source port
            this.DestinationPort = GetUInt16(stream);
            GetBytes(stream, 2); // UDP Length
            this.UDPHeaderChecksum = GetUInt16(stream);

            // Get the data
            int dataLength = this.TotalLength - HeadersLength;
            this.Data = GetBytes(stream, dataLength);

            // Set packet validity flag based on rules
            ValidatePacket();
        }

        // Checks that the packet is correct based on several factors
        private void ValidatePacket()
        {
            bool valid = true;

            // Validate IPv4 and UDP header checksums and set flags
            ValidateChecksums();

            if(!this.SourceIP.Equals(ValidFromAddress))    valid = false;
            if(!this.DestinationIP.Equals(ValidToAddress)) valid = false;
            if(!this.DestinationPort.Equals(ValidToPort))  valid = false;
            if(!this.IPv4HeaderChecksumValid == true)      valid = false;
            if(!this.UDPHeaderChecksumValid  == true)      valid = false;

            this.Valid = valid;
        }

        // Validate and set checksum flags
        private void ValidateChecksums()
        {
            // Validate IPv4 Header checksum
            this.IPv4HeaderChecksumValid = ValidateChecksum();

            // Validate UDP Header checksum
            this.UDPHeaderChecksumValid = ValidateChecksum();
        }

        // Validate a checksum
        private bool ValidateChecksum()
        {
            return false;
        }

        // Consume bytes from a memory stream
        private static byte[] GetBytes(MemoryStream stream, int length)
        {
            // Consume bytes from the memory stream
            byte[] bytes = new byte[length];
            stream.Read(bytes, 0, length);

            return bytes;
        }

        // Consume an unsigned int 16 from memory stream
        private int GetUInt16(MemoryStream stream)
        {
            // Convert to byte array
            byte[] bytes = GetBytes(stream, 2);

            // Reverse bytes if converter is using little endian
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            // Convert byte array to int
            int i = BitConverter.ToUInt16(bytes, 0);

            return i;
        }

        // Returns the formatted source address string from the source address bytes
        private string GetIP(byte[] bytes)
        {
            // Get Source IP
            return $"{bytes[0]}.{bytes[1]}.{bytes[2]}.{bytes[3]}";
        }
    }
}
