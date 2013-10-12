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
            Class2 c2 = new Class2();
            Class1 c1 = new Class1(c2);
            Class2 c3 = new Class2();
            c3 = c2;

            c1.temp.Print("c1 value");

            c2.dict["test"] = 234;
            c2.value = 999;
            c1.temp.Print("c1 value after");
            c3.Print("c3 kprining ");
            Console.ReadLine();
            
        }
    }
}
