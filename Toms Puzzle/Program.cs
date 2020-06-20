using System;
using System.IO;
using System.Text.RegularExpressions;
using Toms_Puzzle.Decoders;
using static Toms_Puzzle.Layers.Layer0;
using static Toms_Puzzle.Layers.Layer1;
using static Toms_Puzzle.Layers.Layer2;
using static Toms_Puzzle.Layers.Layer3;
using static Toms_Puzzle.Layers.Layer4;
using static Toms_Puzzle.Layers.Layer5;

namespace Toms_Puzzle
{
    class Program
    {
        // Tom's Data Onion

        // File variables
        static string Directory = "..\\..\\..\\Data\\";
        private static string[] layerData = { "Layer0.txt", "Layer1.txt", "Layer2.txt", "Layer3.txt", "Layer4.txt", "Layer5.txt", "TheCore.txt" };

        // Decoder
        private static IDecoder puzzleDecoder = null;

        // Constructor
        static Program()
        {
            // Setup the decoder to be used
            puzzleDecoder = DecoderFactory.CreateSimpleBaseLogger();   // SimpleBase decoder
            //puzzleDecoder = DecoderFactory.CreateMyASCII85Decoder(); // My decoder
        }

        // Main entry point
        static void Main(string[] args)
        {
            DecodeLayers();
        }

        // Decode all layers
        private static void DecodeLayers()
        {
            for (int i = 0; i < 2; i++)
                DecodeLayer(i, layerData[i], layerData[i+1], puzzleDecoder);
        }

        // Decode a single layer
        private static void DecodeLayer(int index, string data, string outputData, IDecoder decoder)
        {
            string layer = GetPayload(data);

            string output = "";
            switch(index)
            {
                case 0:
                    output = DecodeLayer0(layer, decoder);
                    break;

                case 1:
                    output = DecodeLayer1(layer, decoder);
                    break;

                case 2:
                    output = DecodeLayer2(layer, decoder);
                    break;

                case 3:
                    output = DecodeLayer3(layer, decoder);
                    break;

                case 4:
                    output = DecodeLayer4(layer, decoder);
                    break;

                case 5:
                    output = DecodeLayer5(layer, decoder);
                    break;
            }

            // Output the entire layer payload, including the plaintext
            File.WriteAllText($"{Directory}{outputData}", output);
        }

        // Get the payload from the layer
        private static string GetPayload(string fileName)
        {
            // Read entire file
            string layer = System.IO.File.ReadAllText($"{Directory}{fileName}");

            // Extract the actual payload
            var payload = Regex.Match(layer, @"<~(.*)~>", RegexOptions.Singleline).Groups[1].Value;
            payload = payload.Replace("\n", "").Replace("\r", "");

            return payload;
        }
    }
}
