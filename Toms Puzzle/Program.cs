using System.IO;
using System.Text.RegularExpressions;
using Toms_Puzzle.Interfaces;
using Toms_Puzzle.Factories;
using Toms_Puzzle.Enumerations;
using System;

namespace Toms_Puzzle
{
    // Tom's Data Onion
    class Program
    {
        // File variables
        static string Directory = "..\\..\\..\\Data\\";
        private static string[] layerData = { "Layer0.txt", "Layer1.txt", "Layer2.txt", "Layer3.txt", "Layer4.txt", "Layer5.txt", "TheCore.txt" };

        // Main entry point
        static void Main(string[] args)
        {
            // Decode all layers
            DecodeLayers();
        }

        // Decode all layers
        private static void DecodeLayers()
        {
            // Create the decoder using the decoder factory
            IDecoder decoder = DecoderFactory.InititalizeDecoder(DecoderEnum.ASCII85Decoder);

            for (int i = 0; i < 6; i++)
            {
                // Get the payload from the file data
                string inputFile = GetPayload(layerData[i]);

                // Input file
                string outputFile = DecodeLayer(i, inputFile, decoder);

                // Output file
                string outputFilename = layerData[i + 1];

                // Output the entire layer payload, including the plaintext
                File.WriteAllText($"{Directory}{outputFilename}", outputFile);
            }
        }

        // Decode a single layer
        private static string DecodeLayer(int index, string payload, IDecoder decoder)
        {
            switch(index)
            {
                // Decode layer using the appropriate layer logic and selected decoder
                case 0:
                    return LayerFactory.InitializeLayer(LayerEnum.Layer0).Decode(payload, decoder);

                case 1:
                    return LayerFactory.InitializeLayer(LayerEnum.Layer1).Decode(payload, decoder);

                case 2:
                    return LayerFactory.InitializeLayer(LayerEnum.Layer2).Decode(payload, decoder);

                case 3:
                    return LayerFactory.InitializeLayer(LayerEnum.Layer3).Decode(payload, decoder);

                case 4:
                    return LayerFactory.InitializeLayer(LayerEnum.Layer4).Decode(payload, decoder);

                case 5:
                    return LayerFactory.InitializeLayer(LayerEnum.Layer5).Decode(payload, decoder);

                default:
                    throw new ArgumentException("Invalid layer index provided");
            }
        }

        // Get the payload from the layer file
        private static string GetPayload(string fileName)
        {
            // Read entire file
            string layer = File.ReadAllText($"{Directory}{fileName}");

            // Extract the actual payload
            var payload = Regex.Match(layer, @"<~(.*)~>", RegexOptions.Singleline).Groups[1].Value;
            payload = payload.Replace("\n", "").Replace("\r", "");

            return payload;
        }
    }
}
