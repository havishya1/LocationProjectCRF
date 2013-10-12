using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class Class2
    {
        public int value;
        public Dictionary<string, int> dict;

        public Class2()
        {
            value = 1;
            dict = new Dictionary<string, int>();
            dict["test"] = 1;
        }

        public void Print(string Mesg)
        {
            Console.WriteLine(Mesg+ "value is: "+value);
            Console.WriteLine(Mesg + "dictionry test is: :"+ dict["test"]);
        }
    }
}
