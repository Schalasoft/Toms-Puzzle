namespace Toms_Puzzle.Decoders
{
    static class DecoderFactory
    {
        public static IDecoder CreateSimpleBaseLogger()
        {
            return new SimpleBaseDecoder();
        }

        public static IDecoder CreateMyASCII85Decoder()
        {
            return new MyASCII85Decoder();
        }
    }
}
