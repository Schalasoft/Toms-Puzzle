using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Toms_Puzzle.Utilities
{
    // IPv4 Packet class
    class Packet
    {
        // Fixed attributes of a packet
        private const int IPHeaderLength = 20;
        private const int UDPHeaderLength = 8;

        // Validation
        private const string ValidFromAddress = "10.1.1.10";
        private const string ValidToAddress = "10.1.1.200";
        private const int ValidToPort = 42069;

        // Properties
        public string SourceIP { get; private set; }
        public string DestinationIP { get; private set; }
        public int DestinationPort { get; private set; }
        public int TotalLength { get; private set; }
        public bool IPHeaderChecksumValid { get; private set; }
        public bool UDPHeaderChecksumValid { get; private set; }
        public byte[] Data { get; private set; }
        public bool Valid { get; private set; }

        // Constructor
        public Packet(MemoryStream stream)
        {
            // Consume the packet
            ConsumePacket(stream);
        }

        // Consume and return a section of bytes from a memory stream
        private byte[] ByteArrayFromMemoryStream(MemoryStream stream, int length)
        {
            // Convert to byte array
            byte[] bytes = new byte[length];
            stream.Read(bytes, 0, length);

            return bytes;
        }

        // Consumes the packet to set the class properties
        private void ConsumePacket(MemoryStream stream)
        {
            // Get IPv4 Header properties (20 bytes)
            byte[] IPHeader = ByteArrayFromMemoryStream(stream, IPHeaderLength); // Store the full header for checksum validation
            MemoryStream IPHeaderStream = new MemoryStream(IPHeader);
            GetBytes(IPHeaderStream, 12); // Version to Checksum (remove bytes we don't care about)
            this.SourceIP = GetIP(GetBytes(IPHeaderStream, 4));
            this.DestinationIP = GetIP(GetBytes(IPHeaderStream, 4));

            // Get UDP Header properties (8 bytes)
            byte[] UDPHeader = ByteArrayFromMemoryStream(stream, UDPHeaderLength);
            MemoryStream UDPHeaderStream = new MemoryStream(UDPHeader);
            GetBytes(UDPHeaderStream, 2); // Source Port
            this.DestinationPort = GetUInt16(UDPHeaderStream);
            int udpTotalLength = GetUInt16(UDPHeaderStream);
            GetBytes(UDPHeaderStream, 2); // UDP Checksum

            // Get the data from the UDP
            int dataLength = udpTotalLength - UDPHeaderLength; // Subtract size of header (8) from the overall size of the header
            this.Data = GetBytes(stream, dataLength);

            // Set packet validity flag based on rules
            ValidatePacket(IPHeader, UDPHeader);
        }

        // Checks that the packet is correct based on several factors
        private void ValidatePacket(byte[] ipHeader, byte[] udpHeader)
        {
            // Default to valid
            bool valid = true;

            // Determine if the packet is invalid
            if (!ValidateChecksums(ipHeader, udpHeader) ||
                !this.SourceIP.Equals(ValidFromAddress) ||
                !this.DestinationIP.Equals(ValidToAddress) ||
                !this.DestinationPort.Equals(ValidToPort) ||
                !this.IPHeaderChecksumValid ||
                !this.UDPHeaderChecksumValid)
                valid = false;

            this.Valid = valid;
        }

        // Validate and set checksum flags
        private bool ValidateChecksums(byte[] ipHeader, byte[] udpHeader)
        {
            // Validate IPv4 Header checksum
            this.IPHeaderChecksumValid = ValidateChecksum(ipHeader);

            // Validate UDP Header checksum
            this.UDPHeaderChecksumValid = ValidateChecksum(udpHeader);

            return IPHeaderChecksumValid && this.UDPHeaderChecksumValid;
        }

        // Validate a checksum 
        // Packet is valid if the result of the ones complement of the sum
        // of each 16 bit value in header (including 2 checksum bytes) is 0
        private bool ValidateChecksum(byte[] header)
        {
            // Convert to stream
            MemoryStream stream = new MemoryStream(header);

            // Consume the stream, summing the values in the header
            int sum = 0;
            while(stream.Position < stream.Length)
                sum += GetUInt16(stream);

            // First byte is the carry bits
            // Get carry value by doing a right shift on the sum to move 
            // the 2 carry bits to the end of the sequence
            // This has the side effect of zeroing bits 0 to 14 so we can add it to sum correctly
            int carry = sum >> 16;

            // Add carry value to the sum
            sum += carry;

            // Ones complement (XOR by max value of UInt16 i.e. all bits are 1 to invert)
            int onesComplement = sum ^ UInt16.MaxValue;

            // If ones complement is 0 then the header is valid
            if (onesComplement == 0)
                return true;
            else
                return false;
        }

        // Consume and return bytes from a memory stream
        private static byte[] GetBytes(MemoryStream stream, int length)
        {
            // Consume bytes from the memory stream
            byte[] bytes = new byte[length];
            stream.Read(bytes, 0, length);

            return bytes;
        }

        // Consume and return an unsigned int 16 from memory stream
        private UInt16 GetUInt16(MemoryStream stream)
        {
            // Convert to byte array
            byte[] bytes = GetBytes(stream, 2);

            // Reverse bytes if converter is using little endian
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            // Convert byte array to int
            UInt16 i = BitConverter.ToUInt16(bytes, 0);

            return i;
        }

        // Returns the formatted IP address string from the IP address bytes
        private string GetIP(byte[] bytes)
        {
            // Get Source IP
            return $"{bytes[0]}.{bytes[1]}.{bytes[2]}.{bytes[3]}";
        }
    }
}
