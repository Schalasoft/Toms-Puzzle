using System;
using Toms_Puzzle.Enumerations;
using Toms_Puzzle.Layers;

namespace Toms_Puzzle.Factories
{
    class LayerFactory
    {
        public static ILayer InitializeLayer(LayerEnum type)
        {
            // Initialize the appropriate decoder
            switch (type)
            {
                case LayerEnum.None:
                    return null;

                case LayerEnum.Layer0:
                    return new Layer0();

                 case LayerEnum.Layer1:
                     return new Layer1();

                case LayerEnum.Layer2:
                    return new Layer2();

                case LayerEnum.Layer3:
                    return new Layer3();

                case LayerEnum.Layer4:
                    return new Layer4();

                case LayerEnum.Layer5:
                    return new Layer5();

                default:
                    throw new ArgumentException("Invalid Layer Enumeration provided");
            }
        }
    }
}
