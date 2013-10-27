using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocationProjectWithFeatureTemplate;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //SecondStep();
            TestMethod1();
            Console.ReadLine();
            
        }

        public static void SecondStep()
        {
            var input = "../../InputRawText/OutputRawText{0}.txt";
            var output = "../../OutputText/InputToCRF{0}";
            for (var i = 1; i < 13; i++)
            {
                var testInput = string.Format(input, i);
                var testOutput = string.Format(output, i);
                Console.WriteLine("processing: "+ testInput);
                ProcessRawText.CreateInputForCRF(testInput, testOutput);
            }
        }


        public static void FirstStep()
        {
            var input = "../../InputRawText/RawText{0}.txt";
            var output = "../../InputRawText/OutputRawText{0}.txt";
            for (var i = 1; i < 14; i++)
            {
                var testInput = string.Format(input, i);
                var testOutput = string.Format(output, i);
                Console.WriteLine("processing: " + testInput);
                var process = new ProcessRawText(testInput, testOutput);
                process.Process();
            }
        }

        public static void TestMethod1()
        {
            var str = new List<string>
            {
                "You",
                "leave",
                "the",
                "Nagpur-Jabbalpur",
                "highway",
                "at",
                "Pawni",
                "and",
                "proceed",
                "to",
                "Sillari."

            };
            var ft = new Features("OTHER", "OTHER", "LOCATION", str, 6);
            foreach (var feat in ft.GetFeatures())
            {
                Console.WriteLine(feat);
            }
        }
    }
}
