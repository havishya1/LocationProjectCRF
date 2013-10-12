using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationProject
{
    class WeightVector
    {
        public readonly Dictionary<string, float> WDictionary;

        public WeightVector()
        {
            WDictionary = new Dictionary<string, float>();
        }
        
        public void Add(KeyValuePair<string, string> input)
        {
            WDictionary.Add(input.Key, float.Parse(input.Value));
        }

        public float Get(string tag)
        {
            return WDictionary.ContainsKey(tag) ? WDictionary[tag] : 0;
        }

        public void AddToKey(string key, float value)
        {
            if (!WDictionary.ContainsKey(key))
            {
                WDictionary.Add(key, 0);
            }

            WDictionary[key] += value;
        }

        public void NormalizeAllWeights(int normalize = 10)
        {
            var tagDictionary = new Dictionary<string, float>();
            foreach (var keyValuePair in WDictionary)
            {
                var tag = keyValuePair.Key.Substring(0, 
                    keyValuePair.Key.IndexOf("TAG:", System.StringComparison.Ordinal));
                if (tagDictionary.ContainsKey(tag))
                {
                    tagDictionary[tag] += Abs(keyValuePair.Value);
                }
                else
                {
                    tagDictionary.Add(tag, Abs(keyValuePair.Value));
                }
            }


            List<string> keyList = WDictionary.Keys.ToList();
            foreach (var key in keyList)
            {
                var tag = key.Substring(0,
                    key.IndexOf("TAG:", StringComparison.Ordinal));

                WDictionary[key] = (WDictionary[key] * normalize) /
                    tagDictionary[tag];
            }
        }

        private float Abs(float value)
        {
            if (value < 0)
            {
                value = -1 * value;
            }
            return value;
        }
    }
}
