using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LocationProjectWithFeatureTemplate
{
    public class ComputeGradient
    {
        public WriteModel Logger { get; set; }
        private readonly List<List<string>> _inputSentence;
        private readonly List<List<string>> _outputTagsList;
        private readonly List<string> _tagList;
        private readonly double _lambda;
        private readonly double _learningParam;
        private readonly FeatureCache _cache;
        private List<ForwardBackwordAlgo> forwardBackwordAlgos;
        private WeightVector _weightVector;
        private string[] _twoGramsList;
        private KeyValuePair<string,string>[] _twoGramPair;
        public Dictionary<string, int> FeatureKDictionary;

        public ComputeGradient(List<List<string>> inputSentence, List<List<string>> tagsList,
            List<string> tagList, double lambda, double learningParam, FeatureCache cache, WriteModel logger)
        {
            Logger = logger;
            _inputSentence = inputSentence;
            _outputTagsList = tagsList;
            _tagList = tagList;
            _lambda = lambda;
            _learningParam = learningParam;
            _cache = cache;
            forwardBackwordAlgos = new List<ForwardBackwordAlgo>();
            _weightVector = null;
            _twoGramsList = new string[4];
            _twoGramPair = new KeyValuePair<string, string>[4];
            var ngramTags = new Tags(_tagList);
            int index = 0;
            foreach (var ngram in ngramTags.GetNGramTags(2))
            {
                if (index >= _twoGramsList.Length)
                {
                    Array.Resize(ref _twoGramsList, index+1);
                    Array.Resize(ref _twoGramPair, index + 1);
                }
                string[] split = ngram.Split(new[] { ':' });
                _twoGramsList[index] = split[0] +"@#"+ split[1];
                _twoGramPair[index] = new KeyValuePair<string, string>(split[0], split[1]);
                index++;
            }
        }

        public void Dump(string outputFile, Dictionary<int, string> dictKtoFeature)
        {
            Console.WriteLine(DateTime.Now+" training is complete");
            var output = new WriteModel(outputFile);
            for (int index = 0; index <_weightVector.FeatureCount; index++)
            {
                output.WriteLine(string.Format("{0} {1} {2}", index,
                    dictKtoFeature[index], _weightVector.WeightArray[index]));
            }
            //var sortedDictionary = from pair in _weightVector.WDictionary
            //    orderby Math.Abs(pair.Value) descending
            //    select pair;
            //foreach (var weight in sortedDictionary)
            //{
            //    output.WriteLine(string.Format("{0} {1} {2}", weight.Key,
            //        dictKtoFeature[weight.Key], weight.Value));
            //}
            output.Flush();
            //Logger.Flush();
        }

        private void SetForwardBackwordAlgo(WeightVector weightVector)
        {
            if (_inputSentence.Count != _outputTagsList.Count)
            {
                throw new Exception("counts dont match " + _inputSentence.Count + "with " + _outputTagsList.Count);
            }
            int counter = 0;
            forwardBackwordAlgos.Clear();
            foreach (var sentence in _inputSentence)
            {
                if (counter % 100 == 0)
                    Console.WriteLine(DateTime.Now + "running fw/backword iteration: "+counter);
                var algo = new ForwardBackwordAlgo(sentence, weightVector, _tagList);
                algo.Run();
                forwardBackwordAlgos.Add(algo);
                counter++;
            }
        }

        public WeightVector RunIterations(WeightVector weightVector, int iterationCount, int threadCount = 1)
        {
            _weightVector = weightVector;
         
            for (var iter = 0; iter < iterationCount; iter++)
            {
                Console.WriteLine(DateTime.Now + " running iteration " + iter);
                
                var newWeightVector = _weightVector.DeepCopy();
                SetForwardBackwordAlgo(newWeightVector);
                if (threadCount > 1)
                {
                    var doneEvents = new ManualResetEvent[threadCount];
                    var partition = newWeightVector.FeatureCount / threadCount;

                    for (int threadIndex = 0; threadIndex < threadCount; threadIndex++)
                    {
                        var start = threadIndex*partition;
                        var end = start + partition;
                        end = end > newWeightVector.FeatureCount ? newWeightVector.FeatureCount : end;
                        doneEvents[threadIndex] = new ManualResetEvent(false);

                        var info = new ThreadInfoObject(this, start, end, newWeightVector,
                            doneEvents[threadIndex], null);
                        ThreadPool.QueueUserWorkItem(info.StartGradientComputing, threadIndex);
                    }

                    WaitHandle.WaitAll(doneEvents);
                }
                else
                {
                    ComputeRange(0, _weightVector.FeatureCount, newWeightVector);
                }
                _weightVector = newWeightVector;
                if (iter + 1 < iterationCount)
                {
                    _weightVector.AvgNormalize();
                }
            }
            _weightVector.AvgNormalize();
            return _weightVector;
        }

        public void ComputeRange(int start, int end, WeightVector newWeightVector, int threadIndex = 0)
        {
            for (var k = start; k < end; k++)
            {
                if (k % 100 == 0)
                {
                    Console.WriteLine(DateTime.Now + "threadIndex: " + threadIndex +
                        " running iteration for k " + k);
                }
                var wk = Compute(k);
                if (double.IsNaN(wk) || double.IsInfinity(wk))
                {
                    Console.WriteLine("k: "+ k + "wk is infiity of nana"+ wk);
                    //Logger.Flush(false);
                }
                newWeightVector.SetKey(k, wk);
            }
        }

        private double Compute(int k)
        {
            BigInteger outputBigInteger = 0;
            double outputDouble = 0;
            int lineIndex = 0;

            if (_inputSentence.Count != _outputTagsList.Count)
            {
                throw new Exception("counts dont match "+ _inputSentence.Count + "with "+ _outputTagsList.Count);
            }
            double firstTerm = 0;
            double secondTerm = 0;
            string kstring = "@#" + k.ToString(CultureInfo.InvariantCulture);
            for (lineIndex = 0; lineIndex< _inputSentence.Count; lineIndex++)
            {
                var outputTags = _outputTagsList[lineIndex];

                double initOutputDouble = 0;
                initOutputDouble += GetAllFeatureKFromCache(outputTags, k, lineIndex);
                initOutputDouble -= CalculateGradient(outputTags, k,
                lineIndex, kstring);

                outputDouble += initOutputDouble;
            }
            //outputDouble = firstTerm - secondTerm;

            var finalOutput = outputDouble - (_lambda * _weightVector.Get(k));
            //Console.WriteLine(k + " updating from " + _weightVector.Get(k) + " finalOutput : " + finalOutput +
            //    "first: "+firstTerm + " second: "+secondTerm);
            finalOutput = _weightVector.Get(k) + (_learningParam * finalOutput);
            return finalOutput;
        }

        private double CalculateGradient(List<string> outputTags,
            int k, int lineIndex, string kString)
        {
            double secondTerm = 0;
            
            // second term.
            for (var pos = 0; pos < outputTags.Count; pos++)
            {
                secondTerm += GetSecondTerm(lineIndex, pos, k, kString);
            }
            return secondTerm;
        }

        private double GetSecondTerm(int lineIndex, int pos, int k, string kString)
        {
            double sum = 0;
            for(var i = 0; i< _twoGramsList.Length; i++)
            {
                if (_cache.Contains(_twoGramsList[i], kString, pos, lineIndex))
                {
                    var value = forwardBackwordAlgos[lineIndex].GetQ(pos, _twoGramPair[i].Key,
                        _twoGramPair[i].Value);
                    //sum += (value * _weightVector.Get(k));
                    sum += value;
                    //if (double.IsNaN(sum) || double.IsInfinity(sum) || double.IsNegativeInfinity(sum))
                    //{
                    //    Console.WriteLine("sum is NAN k:" + k + " weight: " + _weightVector.Get(k) + " value is: " +
                    //                      value +"sum= "+sum);
                    //    //Logger.Flush(false);
                    //}
                }
            }
            return sum;
        }

        public BigInteger GetAllFeatureKFromCacheInBig(List<string> tags, int k, int lineIndex)
        {
            BigInteger sum = 0;
            for (var pos = 0; pos < tags.Count; pos++)
            {
                var prevTag = "*";
                if (pos > 0)
                {
                    prevTag = tags[pos - 1];
                }
                if (_cache.Contains(prevTag, tags[pos], k, pos, lineIndex))
                {
                    var val = Math.Exp(_weightVector.Get(k));
                    if (double.IsInfinity(val))
                    {
                        sum += (BigInteger)_weightVector.Get(k);
                    }
                    else
                    {
                        sum += (BigInteger) val;
                    }

                }
            }
            return sum;
        }

        public double GetAllFeatureKFromCache(List<string> tags, int k, int lineIndex)
        {
            double sum = 0;
            for (var pos = 0; pos < tags.Count; pos++)
            {
                var prevTag = "*";
                if (pos > 0)
                {
                    prevTag = tags[pos - 1];
                }
                if (_cache.Contains(prevTag, tags[pos], k, pos, lineIndex))
                {
                    sum ++;
                }
            }
            return sum;
        }

        public double GetAllFeatureKFromCacheWithWeights(List<string> tags, int k,
            int lineIndex, WeightVector weightVector)
        {
            double sum = 0;
            for (var pos = 0; pos < tags.Count; pos++)
            {
                var prevTag = "*";
                if (pos > 0)
                {
                    prevTag = tags[pos - 1];
                }
                if (_cache.Contains(prevTag, tags[pos], k, pos, lineIndex))
                {
                    //var val = Math.Exp(_weightVector.Get(k));
                    sum += (_weightVector.Get(k));
                }
            }
            return sum;
        }

        public void RunLBFGAlgo(WeightVector weightVector)
        {
            double epsg = 0.0000000001;
            double epsf = 0;
            double epsx = 0;
            int maxits = 50;
            alglib.minlbfgsstate state;
            alglib.minlbfgsreport rep;

            _weightVector = weightVector;
            this.FeatureKDictionary = weightVector.FeatureKDictionary;

            alglib.minlbfgscreate(5, weightVector.WeightArray, out state);
            alglib.minlbfgssetcond(state, epsg, epsf, epsx, maxits);
            alglib.minlbfgsoptimize(state, GetFunctionValueAndGradient, null, this);
            double[] output;
            alglib.minlbfgsresults(state, out output, out rep);
            weightVector.WeightArray = output;

            System.Console.WriteLine(DateTime.Now+": terminationtype {0}", rep.terminationtype); // EXPECTED: 4
            //System.Console.WriteLine("{0}", alglib.ap.format(x, 2)); // EXPECTED: [-3,3]
            //System.Console.ReadLine();
            
        }

        public static void GetFunctionValueAndGradient(double[] weights, ref double func,
            double[] grad, object obj)
        {
            var gradient = (ComputeGradient)obj;
            var weightVector = gradient._weightVector.DeepCopy();
            
            weightVector.WeightArray = weights;
            weightVector.MaxNormalize();
            var count = 0;
            foreach (var weight in weights)
            {
                if (weight > 1)
                {
                    Console.WriteLine(count+ ": before weight "+ weight);
                }
                count++;
            }
            gradient.SetForwardBackwordAlgo(weightVector);
            func = gradient.ComputeFunctionValue(weightVector);
            //gradient.ComputeGradientValues(weightVector, grad, 0, weightVector.FeatureCount);
            gradient.ComputeGradientMultiThread(weightVector, grad, 8);
            count = 0;
            foreach (var weight in weights)
            {
                if (weight > 1)
                {
                    Console.WriteLine(count + ": after weight " + weight);
                }
                count++;
            }
        }

        public double ComputeFunctionValue(WeightVector weightVector)
        {
            double outputDouble = 0;

            for (var lineIndex = 0; lineIndex < _inputSentence.Count; lineIndex++)
            {
                var outputTags = _outputTagsList[lineIndex];

                double initOutputDouble = 0;
                for (var k = 0; k < weightVector.FeatureCount; k++)
                {
                    initOutputDouble += GetAllFeatureKFromCacheWithWeights(outputTags, k,
                        lineIndex, weightVector);
                }
                initOutputDouble -= Math.Log(forwardBackwordAlgos[lineIndex].Z);

                outputDouble += initOutputDouble;
            }
            Console.WriteLine(DateTime.Now+": new function value is: "+outputDouble);
            return -outputDouble;
        }

        public void ComputeGradientValues(WeightVector weightVector, double[] gradient,
            int startIndex, int endIndex )
        {
            for (var k = startIndex; k < endIndex; k++)
            {
                double outputDouble = 0;

                var kstring = "@#" + k.ToString(CultureInfo.InvariantCulture);
                for (var lineIndex = 0; lineIndex < _inputSentence.Count; lineIndex++)
                {
                    var outputTags = _outputTagsList[lineIndex];

                    double initOutputDouble = 0;
                    initOutputDouble += GetAllFeatureKFromCache(outputTags, k, lineIndex);
                    initOutputDouble -= CalculateGradient(outputTags, k,
                        lineIndex, kstring);

                    outputDouble += initOutputDouble;
                }

                gradient[k] = outputDouble - (_lambda * weightVector.Get(k));
            }
        }

        public void ComputeGradientMultiThread(WeightVector weightVector, double[] gradient,
            int threadCount)
        {
            if (threadCount > 1)
            {
                //var newWeightVector = weightVector.DeepCopy();

                var doneEvents = new ManualResetEvent[threadCount];
                var partition = weightVector.FeatureCount / threadCount;

                for (int threadIndex = 0; threadIndex < threadCount; threadIndex++)
                {
                    var start = threadIndex*partition;
                    var end = start + partition;
                    end = end > weightVector.FeatureCount ? weightVector.FeatureCount : end;
                    doneEvents[threadIndex] = new ManualResetEvent(false);

                    var info = new ThreadInfoObject(this, start, end, weightVector,
                        doneEvents[threadIndex], gradient);
                    ThreadPool.QueueUserWorkItem(info.StartLBFGGradientComputing, threadIndex);
                }

                WaitHandle.WaitAll(doneEvents);
            }
            ComputeGradientValues(weightVector, gradient, 0, weightVector.FeatureCount);
        }

    }
    
}

