using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralClassification
{
    class InputDataNormalizer
    {
        public List<double> input;
        private double slope = 0;
        private double intercept = 0;

        public InputDataNormalizer()
        {
            input = new List<double>();
        }

        public void Prepare()
        {
            double maxValue = double.MinValue;
            double minValue = double.MaxValue;

            foreach(double singleValue in input)
            {
                if (singleValue < minValue) minValue = singleValue;
                if (singleValue > maxValue) maxValue = singleValue;
            }

            slope = 2 / (maxValue - minValue);
            intercept = 1.0 - (slope * maxValue);

        }


        public double Normalize(double singleValue)
        {
            double result = slope* singleValue  + intercept;
            return result;
        }
    }
}
