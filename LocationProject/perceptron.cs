using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LocationProject
{
    class Perceptron
    {
        private readonly string _inputFile;
        private readonly string _outputFile;
        private readonly WeightVector _weightVector;
        private readonly ViterbiForGlobalLinearModel _viterbiForGlobalLinearModel;

        public Perceptron(string inputFile, string outputFile, List<string> tagList)
        {
            _inputFile = inputFile;
            _outputFile = outputFile;
            var tags = new Tags(tagList);
            _weightVector = new WeightVector();
            _viterbiForGlobalLinearModel = new ViterbiForGlobalLinearModel(_weightVector, tags);
        }

        public void Train()
        {
            for (var i = 0; i < 5; i++)
            {
                Console.WriteLine("training iteration: "+ i);
                var inputData = new ReadInputData(_inputFile);
                foreach (var line in inputData.GetSentence())
                {
                    var inputTags = new List<string>(line.Count);
                    for(var j = 0; j < line.Count;j++)
                    {
                        var split = line[j].Split(new char[] {' '});
                        line[j] = split[0];
                        inputTags.Add(split[1]);
                    }
                    List<string> temp;
                    var outputTags = _viterbiForGlobalLinearModel.Decode(line, false, out temp);
                    if (Match(inputTags, outputTags)) continue;
                    var inputFeature = (new FeatureWrapper(inputTags, line)).NextFeature().GetEnumerator();
                    var outputFeature= new FeatureWrapper(outputTags, line).NextFeature().GetEnumerator();
                    while (inputFeature.MoveNext() && outputFeature.MoveNext())
                    {
                        if (inputFeature.Current.Key.Equals(outputFeature.Current.Key))
                            continue;
                        _weightVector.AddToKey(inputFeature.Current.Value,
                            1 * Features.GetWeight(inputFeature.Current.Value));
                        _weightVector.AddToKey(outputFeature.Current.Value,
                            -1 * Features.GetWeight(inputFeature.Current.Value));
                    }
                }
                
                inputData.Reset();    
            }

            //  _weightVector.NormalizeAllWeights(100);

            Console.WriteLine("training is complete");
            var output = new WriteModel(_outputFile);
            foreach (var weight in _weightVector.WDictionary)
            {
                output.WriteLine(string.Format("{0} {1}", weight.Key, weight.Value));
            }
            output.Flush();
        }

        private static bool Match(IReadOnlyCollection<string> inputTags, IReadOnlyList<string> outputTags)
        {
            if (inputTags == null) return false;
            if (inputTags.Count != outputTags.Count)
            {
                throw new Exception(inputTags.Count + " don't match " + outputTags.Count);
            }

            return !inputTags.Where((t, i) => !t.Equals(outputTags[i])).Any();
        }
    }
}
