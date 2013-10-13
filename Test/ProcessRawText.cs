using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LocationProjectWithFeatureTemplate;

namespace Test
{
    public class ProcessRawText
    {
        private ReadModel _reader;
        private WriteModel _writer;

        public ProcessRawText(string input, string output)
        {
            _reader = new ReadModel(input);
            _writer = new WriteModel(output);
        }

        public void Process()
        {
            var tags = new List<string> { "LOCATION", "OTHER" };
            var words = new List<string>();


            const string modelFile = "../../../LocationProjectWithFeatureTemplate/"+
                                    "data/training/tag.model.trial1";

            var testGLMViterbi = new TestGLMViterbi(modelFile, "", "", tags);
            testGLMViterbi.Init();

            foreach (var line in _reader.GetNextLine())
            {
                var temp = ReplaceTags(line).Trim();
                if (string.IsNullOrEmpty(temp))
                    continue;
                var splits = line.Split(new[] {' '});

                foreach (var split in splits)
                {
                    if (string.IsNullOrEmpty(split.Trim()))
                        continue;
                    if (!IsSalutationAbbr(split) && split.EndsWith("."))
                    {
                        words.Add(split);
                        var tempList = new List<string>();
                        var outputTags = testGLMViterbi.ViterbiForGLM.DecodeNew(words, false, out tempList);
                        var str = new StringBuilder();
                        for (var i = 0; i < outputTags.Count; i++)
                        {
                            str.Append(words[i]);
                            if (outputTags[i].Equals("LOCATION"))
                            {
                                str.Append("{LOCATION}");
                            }
                            str.Append(" ");
                        }
                        _writer.WriteLine(str.ToString());
                        _writer.WriteLine("");
                        words.Clear();
                    }
                    else
                    {
                        words.Add(split);
                    }
                }
                
            }
            _writer.Flush();
        }

        private bool IsSalutationAbbr(string lastStr)
        {
            if (char.IsUpper(lastStr[0]) &&
                lastStr.Length <= 4)
            {
                if (lastStr.Length >= 3)
                {
                    return char.IsLower(lastStr[lastStr.Length - 2]);
                }
            }
            return false;
        }

        private static string ReplaceTags(string line)
        {
            line = line.Trim();
            if (line.Length > 2)
            {
                if (char.IsDigit(line[0]) && line.Length < 40)
                    return string.Empty;
                if ((line.StartsWith("#") || line.StartsWith("Day")) &&
                    line.Length < 10)
                {
                    return string.Empty;
                }
                if (line.EndsWith(" :") || line.Length < 20)
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
            line = line.Replace("( ", "(");
            line = line.Replace(" )", ")");
            line = line.Replace(" , ", ", ");
            line = line.Replace(" . ", ". ");
            line = Regex.Replace(line, @"\s+", " ");
            line = Regex.Replace(line, @"<.*?>", "");
            line = line.Replace("''", "");
            line = line.Replace("``", "");
            //line = FixComma(line);

            return line;
        }


        internal static void CreateInputForCRF(string input, string output)
        {
            var reader = new ReadModel(input);
            var keyWriter = new WriteModel(string.Concat(output, ".key"));
            var devWriter = new WriteModel(string.Concat(output, ".key.dev"));

            foreach (var line in reader.GetNextLine())
            {
                var words = line.Split(new[] {' '});
                
                if (words.Length < 4)
                    continue;
               
                foreach (var word in words)
                {
                    if (string.IsNullOrEmpty(word.Trim()))
                        continue;

                    if (word.EndsWith("{LOCATION}"))
                    {
                        keyWriter.WriteLine(word.Replace("{LOCATION}", "") + " " + "LOCATION");
                        devWriter.WriteLine(word.Replace("{LOCATION}", ""));
                    }
                    else if (word.EndsWith("{LOCATION}."))
                    {
                        keyWriter.WriteLine(word.Replace("{LOCATION}.", ".") + " " + "LOCATION");
                        devWriter.WriteLine(word.Replace("{LOCATION}.", "."));
                    }
                    else
                    {
                        keyWriter.WriteLine(word + " " + "OTHER");
                        devWriter.WriteLine(word);
                    }
                }
                keyWriter.WriteLine("");
                devWriter.WriteLine("");
            }
            keyWriter.Flush();
            devWriter.Flush();
        }
    }
}
