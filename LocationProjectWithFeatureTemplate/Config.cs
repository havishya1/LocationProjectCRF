using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationProjectWithFeatureTemplate
{
    public class Config
    {
        public HashSet<string> BlackList;
        public HashSet<string> PronounSet;
        public HashSet<string> ConjunctionSet;
        public HashSet<string> VerbSet;
        public HashSet<string> ArticleSet;
        public HashSet<string> PrepositionSet; 

        private static Config instance;

        const string blackList = "D:\\workspace\\reading\\LocationProjectCRF\\LocationProjectWithFeatureTemplate\\data\\BlackList.txt";
        const string PronounList = "D:\\workspace\\reading\\LocationProjectCRF\\LocationProjectWithFeatureTemplate\\data\\PronounList.txt";
        const string ConjuctionList = "D:\\workspace\\reading\\LocationProjectCRF\\LocationProjectWithFeatureTemplate\\data\\ConjuctionList.txt";
        const string VerbList = "D:\\workspace\\reading\\LocationProjectCRF\\LocationProjectWithFeatureTemplate\\data\\VerbList.txt";
        const string ArticleList = "D:\\workspace\\reading\\LocationProjectCRF\\LocationProjectWithFeatureTemplate\\data\\ArticlesList.txt";
        const string PrepositionList = "D:\\workspace\\reading\\LocationProjectCRF\\LocationProjectWithFeatureTemplate\\data\\PrepositionList.txt";

        private Config()
        {
            BlackList = new HashSet<string>();
            var readBlackList = new ReadModel(blackList);
            foreach (var line in readBlackList.GetNextLine())
            {
                var word = line.ToLowerInvariant().Trim();
                if (string.IsNullOrEmpty(word))
                {
                    continue;
                }
                BlackList.Add(word);
            }
            PronounSet = new HashSet<string>();
            var readPronoun = new ReadModel(PronounList);
            foreach (var line in readPronoun.GetNextLine())
            {
                var word = line.ToLowerInvariant().Trim();
                if (string.IsNullOrEmpty(word))
                {
                    continue;
                }
                PronounSet.Add(word);
            }

            ConjunctionSet = new HashSet<string>();
            var readConjunction = new ReadModel(ConjuctionList);
            foreach (var line in readConjunction.GetNextLine())
            {
                var word = line.ToLowerInvariant().Trim();
                if (string.IsNullOrEmpty(word))
                {
                    continue;
                }
                ConjunctionSet.Add(word);
            }

            VerbSet = new HashSet<string>();
            var readVerb = new ReadModel(VerbList);
            foreach (var line in readVerb.GetNextLine())
            {
                var word = line.ToLowerInvariant().Trim();
                if (string.IsNullOrEmpty(word))
                {
                    continue;
                }
                VerbSet.Add(word);
            }

            ArticleSet = new HashSet<string>();
            var readArticle = new ReadModel(ArticleList);
            foreach (var line in readArticle.GetNextLine())
            {
                var word = line.ToLowerInvariant().Trim();
                if (string.IsNullOrEmpty(word))
                {
                    continue;
                }
                ArticleSet.Add(word);
            }

            PrepositionSet = new HashSet<string>();
            var readPreposition = new ReadModel(PrepositionList);
            foreach (var line in readPreposition.GetNextLine())
            {
                var word = line.ToLowerInvariant().Trim();
                if (string.IsNullOrEmpty(word))
                {
                    continue;
                }
                PrepositionSet.Add(word);
            }
        }

        public static Config Instance
        {
            get { return instance ?? (instance = new Config()); }
        }
    }
}
