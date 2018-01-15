using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralClassification
{
    class Configuration
    {
        public static int howManyInputNeurons = 4;
        public static int howManyHiddenNeurons = 5;
        public static int howManyOutputNeurons = 3;


        public static int GetNumberOfWeightsAndBiases()
        {
            int result = 0;
            int howManyWeights = howManyInputNeurons * howManyHiddenNeurons + howManyHiddenNeurons * howManyOutputNeurons;
            int howManyBiases = howManyHiddenNeurons + howManyOutputNeurons;
            result = howManyWeights + howManyBiases;

            return result;
        }
    }
}
