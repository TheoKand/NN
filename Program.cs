using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeuralClassification
{
    class NeuralClassificationProgram
    {
        static Random rnd = null;

        static void Main(string[] args)
        {
            try
            {

                Console.WriteLine("\nBegin neural network classification demo\n");
                Console.WriteLine("Goal is to predict/classify color based on four numeric inputs\n");
                rnd = new Random(159); // 159 makes 'good' output

                Console.WriteLine("Creating 100 lines of raw data");
                string dataFile = "..\\..\\colors.txt";
                MakeData(dataFile, 1000);

                Console.WriteLine("\nFirst few rows of raw data file are:");
                Helpers.ShowTextFile(dataFile, 5);

                double[][] trainMatrix = null;
                double[][] testMatrix = null;
                Console.WriteLine("\nGenerating train and test matrices using an 80%-20% split");
                MakeTrainAndTest(dataFile, out trainMatrix, out testMatrix);

                Console.WriteLine("\nFirst few rows of training matrix are:");
                Helpers.ShowMatrix(trainMatrix, 5);

                Console.WriteLine("\nCreating 4-input 5-hidden 3-output neural network");
                NeuralNetwork nn = new NeuralNetwork(Configuration.howManyInputNeurons , Configuration.howManyHiddenNeurons, Configuration.howManyOutputNeurons);

                Console.WriteLine("Training to find best neural network weights using PSO with cross entropy error");
                double[] bestWeights = nn.Train(trainMatrix);
                Console.WriteLine("\nBest weights found:");
                Helpers.ShowVector(bestWeights, 2, true);

                Console.WriteLine("\nLoading best weights into neural network");
                nn.SetWeights(bestWeights);

                Console.WriteLine("\nAnalyzing the neural network accuracy on the test data\n");
                double accuracy = nn.Test(testMatrix);
                Console.WriteLine("Prediction accuracy = " + accuracy.ToString("F4"));

                Console.WriteLine("\nEnd neural network classification demo\n");
                Console.ReadLine();
        }
            catch (Exception ex)
            {
                Console.WriteLine("Fatal: " + ex.Message);
                Console.ReadLine();
            }
} // Main()

        // --------------------------------------------------------------------------------------------

        static void MakeData(string dataFile, int numLines)
        {
            //double[] weights = new double[] { -0.1, 0.2, -0.3, 0.4, -0.5,
            //                            0.6, -0.7, 0.8, -0.9, 1.0,
            //                            -1.1, 1.2, -1.3, 1.4, -1.5,
            //                            1.6, -1.7, 1.8, -1.9, 2.0,
            //                            -0.5, 0.6, -0.7, 0.8, -0.9,
            //                            1.5, -1.4, 1.3,
            //                            -1.2, 1.1, -1.0,
            //                            0.9, -0.8, 0.7,
            //                            -0.6, 0.5, -0.4,
            //                            0.3, -0.2, 0.1,
            //                            0.1, -0.3, 0.6 };

            #region define weights and biases
            List<double> weightsAndBiases = new List<double>();
            #region input to hidden weights
            weightsAndBiases.Add(-0.1);
            weightsAndBiases.Add(0.2);
            weightsAndBiases.Add(-0.3);
            weightsAndBiases.Add(0.4);
            weightsAndBiases.Add(-0.5);
            weightsAndBiases.Add(0.6);
            weightsAndBiases.Add(-0.7);
            weightsAndBiases.Add(0.8);
            weightsAndBiases.Add(-0.9);
            weightsAndBiases.Add(1.0);
            weightsAndBiases.Add(-1.1);
            weightsAndBiases.Add(1.2);
            weightsAndBiases.Add(-1.3);
            weightsAndBiases.Add(1.4);
            weightsAndBiases.Add(-1.5);
            weightsAndBiases.Add(1.6);
            weightsAndBiases.Add(-1.7);
            weightsAndBiases.Add(1.8);
            weightsAndBiases.Add(-1.9);
            weightsAndBiases.Add(2.0);
            #endregion
            #region input to hidden biases
            weightsAndBiases.Add(-0.5);
            weightsAndBiases.Add(0.6);
            weightsAndBiases.Add(-0.7);
            weightsAndBiases.Add(0.8);
            weightsAndBiases.Add(-0.9);
            #endregion
            #region hidden to output weights
            weightsAndBiases.Add(1.5);
            weightsAndBiases.Add(-1.4);
            weightsAndBiases.Add(1.3);
            weightsAndBiases.Add(-1.2);
            weightsAndBiases.Add(1.1);
            weightsAndBiases.Add(-1.0);
            weightsAndBiases.Add(0.9);
            weightsAndBiases.Add(-0.8);
            weightsAndBiases.Add(0.7);
            weightsAndBiases.Add(-0.6);
            weightsAndBiases.Add(0.5);
            weightsAndBiases.Add(-0.4);
            weightsAndBiases.Add(0.3);
            weightsAndBiases.Add(-0.2);
            weightsAndBiases.Add(0.1);
            #endregion
            #region hidden to output biases
            weightsAndBiases.Add(0.1);
            weightsAndBiases.Add(-0.3);
            weightsAndBiases.Add(0.6);
            #endregion
            #endregion
            double[] weights = weightsAndBiases.ToArray();



            //double[] weights = new double[43];

            NeuralNetwork nn = new NeuralNetwork(Configuration.howManyInputNeurons, Configuration.howManyHiddenNeurons, Configuration.howManyOutputNeurons);
            //nn.SetWeights(weights);
            nn.InitializeWeights();

            FileStream ofs = new FileStream(dataFile, FileMode.Create);
            StreamWriter sw = new StreamWriter(ofs);

            for (int i = 0; i < numLines; ++i)
            {
                double[] inputs = new double[Configuration.howManyInputNeurons];
                for (int j = 0; j < inputs.Length; ++j)
                    inputs[j] = rnd.Next(1, 10);

                double[] outputs = nn.ComputeOutputs(inputs);

                string color = "";
                int idx = Helpers.IndexOfLargest(outputs);
                if (idx == 0) { color = "red"; }
                else if (idx == 1) { color = "green"; }
                else if (idx == 2) { color = "blue"; }

                sw.WriteLine(string.Join(" ", new List<double>(inputs).Select(item => item.ToString("F1")).ToArray()) + " " + color);
            }
            sw.Close(); ofs.Close();

        } // MakeData

        static void MakeTrainAndTest(string file, out double[][] trainMatrix, out double[][] testMatrix)
        {
            int numLines = 0;
            FileStream ifs = new FileStream(file, FileMode.Open);
            StreamReader sr = new StreamReader(ifs);
            while (sr.ReadLine() != null)
                ++numLines;
            sr.Close(); ifs.Close();

            int numTrain = (int)(0.80 * numLines);
            int numTest = numLines - numTrain;

            double[][] allData = new double[numLines][];  // could use Helpers.MakeMatrix here
            for (int i = 0; i < allData.Length; ++i)
                allData[i] = new double[Configuration.howManyInputNeurons+3];               // (x0, x1, x2, x3), (y0, y1, y2)

            string line = "";
            string[] tokens = null;
            ifs = new FileStream(file, FileMode.Open);
            sr = new StreamReader(ifs);
            int row = 0;

            InputDataNormalizer normalizer = new InputDataNormalizer();

            while ((line = sr.ReadLine()) != null)
            {
                tokens = line.Split(' ');

                for(int i=0;i<Configuration.howManyInputNeurons; i++)
                {
                    allData[row][i] = double.Parse(tokens[i]);
                    normalizer.input.Add(allData[row][i]);
                }

                int indexOfFirstEncodedOutputValue = Configuration.howManyInputNeurons;
                int indexOfSecondEncodedOutputValue = Configuration.howManyInputNeurons + 1;
                int indexOfThirdEncodedOutputValue = Configuration.howManyInputNeurons + 2;

                if (tokens[Configuration.howManyInputNeurons] == "red") { allData[row][indexOfFirstEncodedOutputValue] = 1.0; allData[row][indexOfSecondEncodedOutputValue] = 0.0; allData[row][indexOfThirdEncodedOutputValue] = 0.0; }
                else if (tokens[Configuration.howManyInputNeurons] == "green") { allData[row][indexOfFirstEncodedOutputValue] = 0.0; allData[row][indexOfSecondEncodedOutputValue] = 1.0; allData[row][indexOfThirdEncodedOutputValue] = 0.0; }
                else if (tokens[Configuration.howManyInputNeurons] == "blue") { allData[row][indexOfFirstEncodedOutputValue] = 0.0; allData[row][indexOfSecondEncodedOutputValue] = 0.0; allData[row][indexOfThirdEncodedOutputValue] = 1.0; }
                ++row;
            }
            sr.Close(); ifs.Close();


            normalizer.Prepare();
            for (int i=0;i<allData.Length;i++)
            {
                for(int j=0;j< Configuration.howManyInputNeurons; j++)
                {
                    allData[i][j] = normalizer.Normalize(allData[i][j]);
                }
            }

            Helpers.ShuffleRows(allData);

            trainMatrix = Helpers.MakeMatrix(numTrain, Configuration.howManyInputNeurons+3);
            testMatrix = Helpers.MakeMatrix(numTest, Configuration.howManyInputNeurons+3);

            for (int i = 0; i < numTrain; ++i)
            {
                allData[i].CopyTo(trainMatrix[i], 0);
            }

            for (int i = 0; i < numTest; ++i)
            {
                allData[i + numTrain].CopyTo(testMatrix[i], 0);
            }
        } // MakeTrainAndTest

        // --------------------------------------------------------------------------------------------

    } // class NeuralClassificationProgram


} // ns
