using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            SecondStep();
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
    }
}
