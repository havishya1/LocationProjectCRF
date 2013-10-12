using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationProject
{
    class ReadModel
    {
        readonly StreamReader _stream;
        public ReadModel(string file)
        {
            string fileName = file;

            try
            {
                _stream = new StreamReader(fileName);
            }
            catch (Exception)
            {
                Console.WriteLine(fileName + "is not valid");
                throw;
            }
        }

        public IEnumerable<string> GetNextLine()
        {
            string line;
            while ((line = _stream.ReadLine()) != null)
            {
                yield return line.Trim();
            }
        }

        public IEnumerable<KeyValuePair<string, string>> ModelIterator()
        {
            string line;
            while ((line = _stream.ReadLine()) != null)
            {
                line = line.Trim();
                if (string.IsNullOrEmpty(line)) continue;
                string[] str = line.Split(new[]{' '});
                if (str.Length != 2)
                {
                    Console.WriteLine(line + " doesn't have 2 words");
                    throw new Exception();
                }
                else
                {
                    yield return new KeyValuePair<string, string> ( str[0], str[1] );   
                }
            }
        }
    }
}
