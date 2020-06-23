using System.Text;
using Toms_Puzzle.Decoders;

namespace Toms_Puzzle.Layers
{
    // Decode class
    class Layer4
    {
        public static string DecodeLayer4(string payload, IDecoder decoder)
        {
            byte[] output = Encoding.ASCII.GetBytes(payload);

            // Convert to string
            string result = Encoding.ASCII.GetString(output, 0, output.Length);

            return result;
        }
    }

    /*
    The payload for this layer is encoded as a stream of raw
    network data, as if the solution was being received over the
    internet. The data is a series of IPv4 packets with User
    Datagram Protocol (UDP) inside. Extract the payload data
    from inside each packet, and combine them together to form
    the solution.

    Each packet has three segments: the IPv4 header, the UDP
    header, and the data section. So the first 20 bytes of the
    payload will be the IPv4 header of the first packet. The
    next 8 bytes will be the UDP header of the first packet.
    This is followed by a variable-length data section for the
    first packet. After the data section you will find the
    second packet, starting with another 20 byte IPv4 header,
    and so on.

    You will need to read the specifications for IPv4 and UDP in
    order to parse the data. The official specification for IPv4
    is RFC 791 (https://tools.ietf.org/html/rfc791) and for UDP
    it is RFC 768 (https://tools.ietf.org/html/rfc768). The
    Wikipedia pages for these two protocols are also good, and
    probably easier to read than the RFCs.

    However, the payload contains extra packets that are not
    part of the solution. Discard these corrupted and irrelevant
    packets when forming the solution.

    Each valid packet of the solution has the following
    properties. Discard packets that do not have all of these
    properties.
    */

    // IPv4 Packet class
    class Packet
    {
        // Validation
        private const string ValidFromAddress = "10.1.1.10";
        private const string ValidToAddress = "10.1.1.200";
        private const int ValidToPort = 42069;
        public bool IPv4SourceHeaderCorrect { get; private set; }
        public bool UDPHeaderChecksumCorrect{ get; private set; }

        // Properties
        public string SourceIP { get; set; }
        public string DestinationIP { get; set; }
        public int DestinationPort { get; set; }

        // Checks that the packet is correct
        public static bool ValidatePacket(Packet packet)
        {
            bool result = true;

            if(!packet.SourceIP.Equals(ValidFromAddress)) result = false;
            if(!packet.DestinationIP.Equals(ValidToAddress)) result = false;
            if(!packet.DestinationPort.Equals(ValidToPort)) result = false;

            return result;
        }
    }
}
