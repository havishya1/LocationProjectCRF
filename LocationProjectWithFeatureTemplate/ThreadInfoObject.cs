using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LocationProjectWithFeatureTemplate
{
    public class ThreadInfoObject
    {
        public double[] GradientArray;
        public ThreadInfoObject(ComputeGradient cg, int start, int end, WeightVector wc,
            ManualResetEvent resetEvent, double[] gradient)
        {
            GradientArray = gradient;
            Gradient = cg;
            Start = start;
            End = end;
            NewWeightVector = wc;
            ResetEvent = resetEvent;
        }

        public int Start { get; set; }
        public int End { get; set; }
        public WeightVector NewWeightVector { get; set; }
        public ManualResetEvent ResetEvent { get; set; }
        public ComputeGradient Gradient { get; set; }

        public void StartGradientComputing(Object threadContext)
        {
            int threadIndex = (int)threadContext;
            Console.WriteLine("thread {0} started...{1} to {2}", threadIndex, Start, End);
            Gradient.ComputeRange(Start, End, NewWeightVector,threadIndex);
            Console.WriteLine("thread {0} done {1} to {2}", threadIndex, Start, End);

            ResetEvent.Set();
        }

        public void StartLBFGGradientComputing(Object threadContext)
        {
            int threadIndex = (int)threadContext;
            Console.WriteLine("thread {0} started...{1} to {2}", threadIndex, Start, End);
            Gradient.ComputeGradientValues(NewWeightVector, GradientArray, Start, End);
            Console.WriteLine("thread {0} done {1} to {2}", threadIndex, Start, End);

            ResetEvent.Set();
        }
    }
}
