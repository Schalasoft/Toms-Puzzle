using System.IO;
using System.Text.RegularExpressions;
using Toms_Puzzle.Decoders;
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
            for (int i = 0; i < 6; i++)
            {
                // Input file
                string outputData = DecodeLayer(i, layerData[i]);

                // Output file
                string outputFilename = layerData[i + 1];

                // Output the entire layer payload, including the plaintext
                File.WriteAllText($"{Directory}{outputFilename}", outputData);
            }
        }

        // Decode a single layer
        private static string DecodeLayer(int index, string data)
        {
            // Get the payload from the file data
            string payload = GetPayload(data);

            // Create the decoder using the decoder factory
            DecoderFactory decoderFactory = new DecoderFactory();
            IDecoder decoder = decoderFactory.InititalizeDecoder(DecoderEnum.ASCII85Decoder);

            // Create the layer factory
            LayerFactory layerFactory = new LayerFactory();

            switch(index)
            {
                // Decode layer using the appropriate layer logic and selected decoder
                case 0:
                    return layerFactory.InitializeLayer(LayerEnum.Layer0).Decode(payload, decoder);

                case 1:
                    return layerFactory.InitializeLayer(LayerEnum.Layer1).Decode(payload, decoder);

                case 2:
                    return layerFactory.InitializeLayer(LayerEnum.Layer2).Decode(payload, decoder);

                case 3:
                    return layerFactory.InitializeLayer(LayerEnum.Layer3).Decode(payload, decoder);

                case 4:
                    return layerFactory.InitializeLayer(LayerEnum.Layer4).Decode(payload, decoder);

                case 5:
                    return layerFactory.InitializeLayer(LayerEnum.Layer5).Decode(payload, decoder);

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
