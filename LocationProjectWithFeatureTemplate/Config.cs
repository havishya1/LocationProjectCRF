using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationProjectWithFeatureTemplate
{
    public class Config
    {
        public static HashSet<string> BlackList;

        public Config(string blackList)
        {
            BlackList = new HashSet<string>();
            var readBlackList = new ReadModel(blackList);
            foreach (var line in readBlackList.GetNextLine())
            {
                BlackList.Add(line);
            }
        }
    }
}
