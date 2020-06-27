using System;
using Toms_Puzzle.Decoders;
using Toms_Puzzle.Enumerations;
using Toms_Puzzle.Interfaces;

namespace Toms_Puzzle.Factories
{
    class DecoderFactory
    {
        // Set the decoder to be used
        public static IDecoder InititalizeDecoder(DecoderEnum type)
        {
            // Initialize the appropriate decoder
            switch(type)
            {
                case DecoderEnum.None:
                    return null;

                case DecoderEnum.ASCII85Decoder:
                    return new ASCII85Decoder();

                case DecoderEnum.SimpleBaseDecoder:
                    return new SimpleBaseDecoder();

                default:
                    throw new ArgumentException("Invalid Decoder Enumeration provided");
            }
        }
    }
}
