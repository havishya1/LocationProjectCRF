using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using LocationProjectWithFeatureTemplate;

namespace LocationProjectWithFeatureTemplate
{
    class Program
    {
        static void Main(string[] args)
        {
            var tags = new List<string> { "LOCATION", "OTHER" };
            //ReadNewsWireData();
            TrainingTest(tags);
            Test1(tags, false, true);

            //const string modelFile = "../../data/tag.model";
            //const string input = "../../data/gene.test";
            //const string outputFile = "../../data/gene_test.p2.out";

        }

        private static string EvaluateModel(string keyFile, string devFile, string outputDump)
        {
            var evalModel = new EvalModel();
            return evalModel.Evalulate(keyFile, devFile, outputDump);
        }


        static void TrainingTest(List<string> tags)
        {
            //const string modelFile = "../../data/gene.key.model";
            //const string input = "../../data/gene.key";
            var inputFiles = new[]
            {
                "../../data/training/NYT_19980403_parsed.key",
                "../../data/training/APW_19980314_parsed.key",
                "../../data/training/APW_19980424_parsed.key",
                "../../data/training/APW_19980429_parsed.key",
                "../../data/training/NYT_19980315_parsed.key",
                "../../data/training/NYT_19980407_parsed.key",
                "../../data/travelTraining/InputToCRF1.key",
                "../../data/travelTraining/InputToCRF2.key",
                "../../data/travelTraining/InputToCRF3.key",
                "../../data/travelTraining/InputToCRF4.key",
                "../../data/travelTraining/InputToCRF5.key",
                "../../data/travelTraining/InputToCRF6.key",
                "../../data/travelTraining/InputToCRF7.key",
                "../../data/travelTraining/InputToCRF8.key",
                "../../data/travelTraining/InputToCRF9.key",
                "../../data/travelTraining/InputToCRF10.key",
                //"../../data/travelTraining/InputToCRF11.key",
                //"../../data/travelTraining/InputToCRF12.key",
            };
            const string modelFile = "../../data/tag.model.withoutsecondPassImpFor407";
            const string input = "../../data/training/NYT_19980403_parsed.key";
            string LoggerFile = "../../Logs/Log_"+DateTime.Now.ToFileTime()+".txt";
            const int threadCount = 8;
            var perceptron = new Perceptron(inputFiles.ToList(), modelFile, tags, false);
            perceptron.Train(inputFiles);
            perceptron.ReMapFeatureToK(true);
            perceptron.Dump();
            perceptron.MapFeatures.Dump();

            //ComputeGradient gradient = null;
            //var logger = new WriteModel(LoggerFile);
            //for (int i = 0; i < inputFiles.Length; i++)
            //{
            //    Console.WriteLine("running for input["+i+"]: "+ input[i]);
            //    perceptron.ReadInputs(inputFiles[i]);
            //    var featureCache = new FeatureCache(perceptron.InputSentences, tags,
            //        perceptron.MapFeatures.DictFeaturesToK);
            //    featureCache.CreateCache();
                
            //    const double lambda = 0;
            //    const double learningParam = .1;
            //    gradient = new ComputeGradient(perceptron.InputSentences, perceptron.TagsList,
            //        tags, lambda, learningParam, featureCache, logger);
            //    //perceptron.WeightVector.ResetAllToZero();
            //    //gradient.RunIterations(perceptron.WeightVector, 10, threadCount);
            //    gradient.RunLBFGAlgo(perceptron.WeightVector);
            //}
            //if (gradient != null)
            //{
            //    gradient.Dump(modelFile, perceptron.MapFeatures.DictKToFeatures);
            //}

        }

        static void Test1(List<string> tags, bool debug, bool eval)
        {
            //const string input = "../../data/gene.dev";
            //const string outputFile = "../../data/gene_dev.output3";
            //const string modelFile = "../../data/gene.key.model";

            var inputFiles = new[]
                                 {
                                    "../../data/training/NYT_19980403_parsed",
                                    "../../data/training/APW_19980314_parsed",
                                    "../../data/training/APW_19980424_parsed",
                                    "../../data/training/APW_19980429_parsed",
                                    "../../data/training/NYT_19980315_parsed",
                                    "../../data/training/NYT_19980407_parsed",
                                    "../../data/travelTraining/InputToCRF6",
                                    "../../data/travelTraining/InputToCRF7",
                                    "../../data/travelTraining/InputToCRF8",
                                    "../../data/travelTraining/InputToCRF9",
                                    "../../data/travelTraining/InputToCRF10",
                                    "../../data/travelTraining/InputToCRF11",
                                    "../../data/travelTraining/InputToCRF12",
                                 };

            foreach (var inputFile in inputFiles)
            {
                string input = inputFile + ".key.dev";
                string outputFile = inputFile + ".dev.output1";
                string keyFile = inputFile + ".key";
                string outputEval = inputFile + ".dev.evalDump";
                const string modelFile = "../../data/tag.model.withoutsecondPassImpFor407";

                var testGLMViterbi = new TestGLMViterbi(modelFile, input, outputFile, tags);
                testGLMViterbi.Setup(debug);

                if (eval && !debug)
                {
                    var dump = EvaluateModel(keyFile, outputFile, outputEval);
                    Console.WriteLine("training for: "+ inputFile);
                    Console.WriteLine(dump);
                    Console.ReadLine();
                }    
            }
            
        }

        private static void ReadNewsWireData()
        {
            string[] input =
            {
                "../../data/training/APW_19980314",
                "../../data/training/APW_19980424",
                "../../data/training/APW_19980429",
                "../../data/training/NYT_19980315",
                "../../data/training/NYT_19980403",
                "../../data/training/NYT_19980407"
            };
            string[] output =
            {
                "../../data/training/APW_19980314_parsed.key",
                "../../data/training/APW_19980424_parsed.key",
                "../../data/training/APW_19980429_parsed.key",
                "../../data/training/NYT_19980315_parsed.key",
                "../../data/training/NYT_19980403_parsed.key",
                "../../data/training/NYT_19980407_parsed.key"
            };
            //const string input = "../../data/training/APW_19980314";
            //const string output = "../../data/training/APW_19980314.parsed_key";
            for (int i = 0; i < input.Length; i++)
            {
                var parseNEWSWIRE = new ParseNEWSWIRETrainingData();
                parseNEWSWIRE.Parse(input[i], output[i]);
            }
        }

        /*
                static void Test()
                {
                    const string inputFile = "../../data/tag.model";
                    //const string outputFile = "../../test.output1";
                    var readModel = new ReadModel(inputFile);
                    //var writeModel = new WriteModel(outputFile);
                    var weightVector = new WeightVector();
                    var tags = new List<string> {"I-GENE", "O"};

                    PrintFeatureList(tags);

                    foreach (var pair in readModel.ModelIterator())
                    {
                        weightVector.Add(pair);
                
                    }
                    //writeModel.WriteLine(line);
                    //writeModel.Flush();
                }
        */

        static void PrintFeatureList(List<string> tags)
        {
            var featureTags = new Tags(tags);
            featureTags.Dump(3);
        }
    }
}
