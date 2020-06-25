namespace Toms_Puzzle.Decoders
{
    static class DecoderFactory
    {
        public static IDecoder CreateSimpleBaseDecoder()
        {
            return new SimpleBaseDecoder();
        }

        public static IDecoder CreateASCII85Decoder()
        {
            return new ASCII85Decoder();
        }
    }
}
