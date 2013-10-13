using System.Collections.Generic;
using System.Linq;

namespace LocationProjectWithFeatureTemplate
{
    public class TestGLMViterbi
    {
        private readonly string _outputTestFile;
        private readonly List<string> _tagList;
        public string InputModelFile { get; set; }
        public string InputTestFile { get; set; }
        private WeightVector _weightVector;
        private Tags _tags;
        public ViterbiForGlobalLinearModel ViterbiForGLM;

        public TestGLMViterbi(string inputModelFile , string inputTestFile, string outputTestFile, List<string> tagList)
        {
            _outputTestFile = outputTestFile;
            _tagList = tagList;
            InputModelFile = inputModelFile;
            InputTestFile = inputTestFile;
        }

        public void Init()
        {
            var readModel = new ReadModel(InputModelFile);
            var temp = new ReadModel(string.Concat(InputModelFile, ".featuresToK"));
            var dict = temp.GetFeatureToKdDictionary();
            _weightVector = new WeightVector(dict, dict.Count);

            foreach (var pair in readModel.ModelIterator())
            {
                _weightVector.Add(pair);
            }

            _tags = new Tags(_tagList);

            ViterbiForGLM = new ViterbiForGlobalLinearModel(_weightVector, _tags);
        }

        public void Setup(bool debug)
        {

            Init();
            // read input file in a class and per line iterator.
            var inputData = new ReadInputData(InputTestFile);
            var writeModel = new WriteModel(_outputTestFile);
            foreach (var line in inputData.GetSentence())
            {
                List<string> debugList;
                var outputTags = ViterbiForGLM.DecodeNew(line, debug, out debugList);
                if (debug)
                {
                    writeModel.WriteDataWithTagDebug(line, outputTags, debugList);
                }
                else
                {
                    writeModel.WriteDataWithTag(line, outputTags);    
                }
                
            }
            writeModel.Flush();
        }
    }
}
