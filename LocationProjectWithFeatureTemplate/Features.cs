using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace LocationProjectWithFeatureTemplate
{
    class Features
    {
        public string T2 { get; set; }
        public string T1 { get; set; }
        public string T { get; set; }
        public List<string> Sentence { get; set; }
        public int Pos { get; set; }
        private readonly List<FeatureEnums> _featureList;
        private string _currentWord;
        private string _nextWord;
        private string _prevWord;
        private bool _currentStartsWithCap;
        private bool _nextStartsWithCap;
        private bool _prevStartsWithCap;

        enum FeatureEnums
        {
           TRIGRAMTags,
           BiWordTag,
           CurWordPrevTag,
           BiGram,
           Suff1Tag,
           Suff2Tag,
           Suff3Tag,
           AlphaNumTag,
           AllCapsTag,
           AllSmallTag,
           StartWithCapTag,
           SingleCharTag,
           StartsWithNum,
           AllNumTag,
           ContainsSymbolsTag,
           FirstStringTag,
           PrevWordCurrentTag,
           PrevOtherWordCurrentTag,
           NextWordCurrentTag,
           IsPreviousWordSalutatation,
           IsPreviousLocationAndCurrentStartsWithCapital,
           IsPreviousOtherCapitalAndCurrentStartsWithCapital,
           EndsWithApostrophy,
           FirstWord,
            IsSalutation,
           IsNumFollowedByCase,
           ThreeCapitalWords,
           AllCapsOnlyOne,     
           IsBlackListed,
           CurrentLocNextStartCap,
           EndswithLy,
           EndsWithSorEd,
           PronounWord,
           ConjunctionWord,
           VerbWord,
           SandEsFollowedByTag,
           VerbFollowedByTag,
           EndsWithColon,
           EndsWithLL,
           PreceedsByA,
           PrepositionNextTag,
           ConjunctionNextTag,
           ConjunctionPrevTag,
           SuffixNextTag,
           AdjectiveNextTag,
           VerbProceedByTag,
        }

        public Features(string t2, string t1, string t, List<string> sentence, int pos)
        {
            T2 = t2;
            T1 = t1;
            T = t;
            Sentence = sentence;
            Pos = pos;
            _currentWord = sentence[Pos].ToLowerInvariant().Trim();
            _currentStartsWithCap = char.IsUpper(Sentence[Pos][0]);
            if (pos < sentence.Count - 1)
            {
                _nextWord = sentence[Pos + 1].ToLowerInvariant().Trim();
                _nextStartsWithCap = char.IsUpper(Sentence[Pos + 1][0]);
            }
            if (Pos > 0)
            {
                _prevWord = sentence[Pos - 1].ToLowerInvariant().Trim();
                _prevStartsWithCap = char.IsUpper(Sentence[Pos - 1][0]);
            }
            _featureList = new List<FeatureEnums>
            {
                //FeatureEnums.TRIGRAMTags,
                //FeatureEnums.BiGram,
                //FeatureEnums.CurWordPrevTag,
                FeatureEnums.BiWordTag,
                //FeatureEnums.Suff2Tag,
                FeatureEnums.Suff3Tag,
                FeatureEnums.SingleCharTag,
                FeatureEnums.AllCapsTag,
                FeatureEnums.AllSmallTag,
                FeatureEnums.AlphaNumTag,
                FeatureEnums.StartWithCapTag,
                FeatureEnums.StartsWithNum,
                FeatureEnums.AllNumTag,
                FeatureEnums.ContainsSymbolsTag,
                FeatureEnums.FirstStringTag,
                //FeatureEnums.PrevWordCurrentTag,
                //FeatureEnums.NextWordCurrentTag,
                FeatureEnums.PrevOtherWordCurrentTag,
                FeatureEnums.IsPreviousWordSalutatation,
                FeatureEnums.IsPreviousLocationAndCurrentStartsWithCapital,
                FeatureEnums.FirstWord,
                FeatureEnums.IsSalutation,
                FeatureEnums.IsNumFollowedByCase,
                FeatureEnums.ThreeCapitalWords,
                FeatureEnums.AllCapsOnlyOne,
                FeatureEnums.IsBlackListed,
                FeatureEnums.CurrentLocNextStartCap,
                FeatureEnums.EndswithLy,
                FeatureEnums.EndsWithSorEd,
                FeatureEnums.ConjunctionWord,
                FeatureEnums.PronounWord,
                FeatureEnums.VerbWord,
                FeatureEnums.SandEsFollowedByTag,
                FeatureEnums.VerbFollowedByTag,
                FeatureEnums.EndsWithColon,
                //FeatureEnums.EndsWithApostrophy
                FeatureEnums.EndsWithLL,
                FeatureEnums.PreceedsByA,
                FeatureEnums.PrepositionNextTag,
                FeatureEnums.ConjunctionNextTag,
                FeatureEnums.ConjunctionPrevTag,
                //FeatureEnums.SuffixNextTag,
                FeatureEnums.AdjectiveNextTag,
                FeatureEnums.VerbProceedByTag,
            };
        }

        public IEnumerable<string>  GetFeatures()
        {
            foreach (var feature in _featureList)
            {
                switch (feature)
                {   
                    //case FeatureEnums.TRIGRAMTags:
                    //    yield return GetTriGramFeature();
                    //    break;
                    case FeatureEnums.BiWordTag:
                        if (Pos < Sentence.Count)
                        {
                            var tag = GetTagFeature();
                            if (!string.IsNullOrEmpty(tag))
                            {
                                yield return tag;
                            }
                        }
                        break;
                    case FeatureEnums.CurWordPrevTag:
                    {
                        if (Pos < Sentence.Count)
                        {
                            yield return GetCurrentWordPrevTag();
                        }
                        break;                        
                    }

                    case FeatureEnums.BiGram:
                    {
                        yield return GetBiGram();
                        break;
                    }

                    case FeatureEnums.Suff2Tag:
                    {
                        string tag = GetSuffixTag(2);
                        if (!string.IsNullOrEmpty(tag))
                        {
                            yield return tag;
                        }
                        break;
                    }

                    case FeatureEnums.Suff1Tag:
                    {
                        string tag = GetSuffixTag(1);
                        if (!string.IsNullOrEmpty(tag))
                        {
                            yield return tag;
                        }
                        break;
                    }

                    case FeatureEnums.Suff3Tag:
                    {
                        string tag = GetSuffixTag(3);
                        if (!string.IsNullOrEmpty(tag))
                        {
                            yield return tag;
                        }
                        break;
                    }
                    case FeatureEnums.SingleCharTag:
                    {
                        string tag = GetOneLength();
                        if (!string.IsNullOrEmpty(tag))
                        {
                            yield return tag;
                        }
                        break;
                    }
                    case FeatureEnums.AllCapsTag:
                    {
                        string tag = GetAllCapsTag();
                        if (!string.IsNullOrEmpty(tag))
                        {
                            yield return tag;
                        }
                        break;
                    }

                    case FeatureEnums.AllSmallTag:
                    {
                        string tag = GetAllSmallTag();
                        if (!string.IsNullOrEmpty(tag))
                        {
                            yield return tag;
                        }
                        break;
                    }
                    case FeatureEnums.AllNumTag:
                    {
                        string tag = GetNumTag();
                        if (!string.IsNullOrEmpty(tag))
                        {
                            yield return tag;
                        }
                        break;
                    }

                    case FeatureEnums.AlphaNumTag:
                    {
                        string tag = GetAlphaNumTag();
                        if (!string.IsNullOrEmpty(tag))
                        {
                            yield return tag;
                        }
                        break;
                    }

                    case FeatureEnums.StartWithCapTag:
                    {
                        string tag = GetStartWithCap();
                        if (!string.IsNullOrEmpty(tag))
                        {
                            yield return tag;
                        }
                        break;
                    }

                    case FeatureEnums.StartsWithNum:
                    {
                        string tag = GetStartWithNum();
                        if (!string.IsNullOrEmpty(tag))
                        {
                            yield return tag;
                        }
                        break;
                    }

                    case FeatureEnums.PrevWordCurrentTag:
                    {
                        string tag = GetPrevWordCurrentTag();
                        if (!string.IsNullOrEmpty(tag))
                        {
                            yield return tag;
                        }
                        break;
                    }
                    case FeatureEnums.PrevOtherWordCurrentTag:
                    {
                        string tag = GetPrevOtherWordCurrentTag();
                        if (!string.IsNullOrEmpty(tag))
                        {
                            yield return tag;
                        }
                        break;
                    }    
                    case FeatureEnums.NextWordCurrentTag:
                    {
                        string tag = GetNextWordCurrentTag();
                        if (!string.IsNullOrEmpty(tag))
                        {
                            yield return tag;
                        }
                        break;
                    }

                    case FeatureEnums.IsPreviousWordSalutatation:
                    {
                        string tag = GetSalutationTag();
                        if (!string.IsNullOrEmpty(tag))
                        {
                            yield return tag;
                        }
                        
                    }
                        break;
                    case FeatureEnums.IsPreviousLocationAndCurrentStartsWithCapital:
                    {
                        string tag = GetPrevLocationAndCurrentCapital();
                        if (!string.IsNullOrEmpty(tag))
                        {
                            yield return tag;
                        }
                        break;
                    }
                    case FeatureEnums.EndsWithApostrophy:
                    {
                        var tag = GetEndsWithApostrophyTag();
                        if (!string.IsNullOrEmpty(tag))
                        {
                            yield return tag;
                        }
                        break;
                    }
                    case FeatureEnums.FirstWord:
                    {
                        if (Pos == 0)
                        {
                            yield return "FIRSTWORDTAG:1:"+ T;
                        }
                        break;
                    }
                    case FeatureEnums.IsPreviousOtherCapitalAndCurrentStartsWithCapital:
                    {
                        string tag = GetPrevCapOtherAndCurrentCapital();
                        if (!string.IsNullOrEmpty(tag))
                        {
                            yield return tag;
                        }
                        break;
                    }
                    case FeatureEnums.IsSalutation:
                    {
                        if (Pos < Sentence.Count && IsSalutationAbbr(Sentence[Pos]))
                        {
                            yield return "SALUTAG:"+Sentence[Pos] +":"+ T;
                        }
                        break;
                    }
                    case FeatureEnums.IsNumFollowedByCase:
                    {
                        var tag = GetNumFollowedByCase();
                        if (!string.IsNullOrEmpty(tag))
                        {
                            yield return tag;
                        }
                        break;
                    }
                    
                    case FeatureEnums.ThreeCapitalWords:
                    {
                        var tag = GetThreeCaptitalWords();
                        if (!string.IsNullOrEmpty(tag))
                        {
                            yield return tag;
                        }
                        break;
                    }
                    case FeatureEnums.AllCapsOnlyOne:
                    {
                        var tag = AllCapsOnlyOne();
                        if (!string.IsNullOrEmpty(tag))
                        {
                            yield return tag;
                        }
                        break;
                    }
                    case FeatureEnums.IsBlackListed:
                    {
                        var word = RemoveSymbols(_currentWord);
                        if (Config.Instance.BlackList.Contains(word))
                        {
                            yield return "BLACKLISTED:1:" + T;
                        }
                        break;
                    }
                    case FeatureEnums.CurrentLocNextStartCap:
                    {
                        if (Pos < Sentence.Count - 1)
                        {
                            if (T.Equals("LOCATION") && _nextStartsWithCap)
                            {
                                yield return "CURLOCNEXTCAP:" + _nextWord + ":" + T;
                            }
                        }
                        break;
                    }
                    case FeatureEnums.EndswithLy:
                    {
                        if (_currentWord.EndsWith("ly"))
                        {
                            // possibly adverb
                            yield return "ENDSWITHLY:1:" + T;
                        }
                        break;
                    }
                    case FeatureEnums.EndsWithSorEd:
                    {
                        if (_currentWord.EndsWith("es") ||
                            _currentWord.EndsWith("ed"))
                        {
                            yield return "ENDSWITHSORED:1:" + T;
                        }
                        break;
                    }
                    case FeatureEnums.ConjunctionWord:
                    {
                        if (Config.Instance.ConjunctionSet.Contains(_currentWord))
                        {
                            yield return  "CONJUCTION:1:" + T;
                        }
                        break;
                    }
                    case FeatureEnums.VerbWord:
                    {
                        if (Config.Instance.VerbSet.Contains(_currentWord))
                        {
                            yield return "VERBSET:1:" + T;
                        }
                        break;
                    }
                    case FeatureEnums.PronounWord:
                    {
                        if (Config.Instance.PronounSet.Contains(_currentWord))
                        {
                            yield return "PRONOUN:1:" + T;
                        }
                        break;
                    }
                    case FeatureEnums.SandEsFollowedByTag:
                    {
                        if (Pos > 0 && _currentStartsWithCap)
                        {
                            if (_prevWord.EndsWith("ed") ||
                                _prevWord.EndsWith("es"))
                            {
                                yield return "SANDESFOLCAPS:1:" + T;
                            }
                        }
                        break;
                    }
                    case FeatureEnums.VerbFollowedByTag:
                    {
                        if (Pos> 0 && _currentStartsWithCap)
                        {
                            if (Config.Instance.VerbSet.Contains(_prevWord))
                            {
                                yield return "VERBFOLCAPS:"+_prevWord+":" + T;
                            }
                        }
                        break;
                    }

                    case FeatureEnums.VerbProceedByTag:
                    {
                        if (Pos < Sentence.Count - 2 && _currentStartsWithCap)
                        {
                            if (Config.Instance.VerbSet.Contains(_nextWord))
                            {
                                yield return "VERBPROCEEDCAPS:"+_nextWord+ ":" + T;
                            }
                            else
                            {
                                var word = Sentence[Pos + 2].ToLowerInvariant().Trim();
                                if (_nextStartsWithCap &&
                                    (Config.Instance.VerbSet.Contains(word)))
                                {
                                    yield return "VERBPROCEEDCAPS:" + word + ":" + T;
                                }    
                            }
                        }
                        break;
                    }

                    case FeatureEnums.EndsWithColon:
                    {
                        if (_currentWord.EndsWith(":"))
                        {
                            yield return "ENDSWITHCOLON:1:" + T;
                        }
                        break;
                    }
                    case FeatureEnums.EndsWithLL:
                    {
                        if (_currentWord.EndsWith("'ll"))
                        {
                            yield return "ENDSWITHLL:1:" + T;
                        }
                        break;
                    }
                    case FeatureEnums.PreceedsByA:
                    {
                        if (Pos > 0 && _currentStartsWithCap)
                        {
                            if (_prevWord.Equals("a") || _prevWord.Equals("an"))
                            {
                                yield return "PRECEEDSA:1:" + T;
                            }
                        }
                        break;
                    }
                    case FeatureEnums.PrepositionNextTag:
                    {
                        if (Pos > 0 && _currentStartsWithCap)
                        {
                            var word = RemoveSymbols(_prevWord);
                            if (Config.Instance.PrepositionSet.Contains(word))
                            {
                                yield return "PREPOSITIONFOLLOW:" + word + ":" + T;
                            }
                        }
                        break;
                    }
                    case FeatureEnums.ConjunctionNextTag:
                    {
                        if (Pos > 0 && _currentStartsWithCap)
                        {
                            if (Config.Instance.ConjunctionSet.Contains(_prevWord))
                            {
                                yield return "CONJUNCNEXTTAG:" + _prevWord + ":" + T;
                            }
                        }
                        break;
                    }

                    case FeatureEnums.ConjunctionPrevTag:
                    {
                        if (Pos < Sentence.Count - 2 && _currentStartsWithCap)
                        {
                            if (Config.Instance.ConjunctionSet.Contains(_nextWord))
                            {
                                yield return "CONJUNCPRETAG:" + _nextWord + ":" + T;
                            }
                            var word = Sentence[Pos + 2].ToLowerInvariant().Trim();
                            if (char.IsUpper(Sentence[Pos][0]) &&
                                char.IsUpper(Sentence[Pos+1][0]) &&
                                Config.Instance.ConjunctionSet.Contains(word))
                            {
                                yield return "CONJUNCPRETAG:" + word + ":" + T;
                            }
                        }
                        break;
                    }
                    case FeatureEnums.SuffixNextTag:
                    {
                        if (Pos > 0 && _prevWord.Length > 3 && _currentStartsWithCap)
                        {
                            var suffixSet = Config.Instance.SuffixSet;
                            var length = _prevWord.Length;
                            for (int i = 1; (i < 9) && (length > i); i++)
                            {
                                var suffix = _prevWord.Substring(length - i, i);
                                if (suffixSet.Contains(suffix))
                                {
                                    yield return "SUFFNEXTTAG:" + suffix + ":" + T;
                                }
                            }
                        }
                        break;
                    }
                       
                    case FeatureEnums.AdjectiveNextTag:
                    {
                        if (Pos > 0)
                        {
                            if (_currentStartsWithCap &&
                                Config.Instance.AdjectiveSet.Contains(_prevWord))
                            {
                                yield return "ADJECNEXTTAG:" + _prevWord + ":" + T;
                            }
                        }
                        break;
                    }
                }
            }
        }

        private string AllCapsOnlyOne()
        {
            if (Pos > 0)
            {
                if (IsAllUpper(Sentence[Pos -1]))
                {
                    return null;
                }
            }
            if (Pos + 1 < Sentence.Count)
            {
                if (IsAllUpper(Sentence[Pos + 1]))
                {
                    return null;
                }
            }
            if (IsAllUpper(Sentence[Pos]))
            {
                return "ALLCAPSONLYONE:1:" + T;
            }
            return null;
        }

        private string GetThreeCaptitalWords()
        {
            if (Pos + 2 < Sentence.Count)
            {
                if (IsAllUpper(Sentence[Pos]))
                    return null;
                if (char.IsUpper(Sentence[Pos][0]) &&
                    (char.IsUpper(Sentence[Pos + 1][0]) || Sentence[Pos+1].Length < 4))
                {
                    if (char.IsUpper(Sentence[Pos + 2][0]))
                    {
                        return "THREECAPWORDS:1:" + T;
                    }
                    else if (Sentence[Pos + 2].Length < 4 &&
                             Pos + 3 < Sentence.Count &&
                             char.IsUpper(Sentence[Pos + 2][0]))
                    {
                        return "THREECAPWORDS:1:" + T;   
                    }
                }
            }
            return null;
        }

        private string GetNumFollowedByCase()
        {
            if (Pos < Sentence.Count - 1)
            {
                var input = Sentence[Pos];
                input = RemoveApos(input);
                input = RemoveSymbols(input);
                int result = 0;
                int caseFlag = char.IsLower(Sentence[Pos + 1][0]) ? 0 : 1;

                if (Int32.TryParse(input, out result))
                {
                    return "NUMFOLLOWCASE:" + caseFlag + ":" + T;
                }
                double temp = 0;
                if (double.TryParse(input, out temp))
                {
                    return "NUMFOLLOWCASE:" + caseFlag + ":" + T;
                }
            }
            return null;
        }

        private string GetPrevCapOtherAndCurrentCapital()
        {
            if (Pos > 0)
            {
                if (T1.Equals("Other") && char.IsUpper(Sentence[Pos][0]) && char.IsUpper(Sentence[Pos-1][0]))
                {
                    return "PREVOTHERCC:1:" + T;
                }
            }
            return null;
        }

        private string GetEndsWithApostrophyTag()
        {
            bool containsApos = ContainsApos(Sentence[Pos]);
            if (!containsApos && Pos + 1 < Sentence.Count)
            {
                containsApos = ContainsApos(Sentence[Pos + 1]);
            }
            if (containsApos)
            {
                return "CONTAINSAPOS:1:" + T;
            }
            return null;
        }

        private bool ContainsApos(string input)
        {
            return input.EndsWith("'s") || input.EndsWith("'S");
        }

        private string RemoveApos(string input)
        {
            if (input.EndsWith("'s") || input.EndsWith("'S"))
            {
                return input.Substring(0, input.Length - 2);
            }
            return input;
        }

        private string GetAllSmallTag()
        {
            if (Pos >= Sentence.Count)
            {
                return null;
            }
            // University of Washington
            if (IsAllSmall(Sentence[Pos]))
            {
                //var word = Sentence[Pos];
                if (T1.Equals("OTHER") || _currentWord.Length > 3 || Pos == Sentence.Count - 1)
                {
                    return "ALLSMALLTAG:1:" + T;
                }
                if (Config.Instance.VerbSet.Contains(_currentWord) ||
                    Config.Instance.ArticleSet.Contains(_currentWord)) 
                {
                    if (Pos < Sentence.Count - 1)
                    {
                        if (char.IsLower(Sentence[Pos + 1][0]) ||
                            IsAllNum(Sentence[Pos+1]))
                        {
                            return "ALLSMALLTAG:1:" + T; 
                        }
                    }
                    else
                    {
                        return "ALLSMALLTAG:1:" + T; 
                    }
                }
                var word = RemoveSymbols(_currentWord);
                Int64 result;
                if (Int64.TryParse(word, out result))
                {
                    return "ALLSMALLTAG:1:" + T;
                }
            }
               
            //return "ALLUPPERTAG:0:" + T;
            return null;
        }

        private string GetPrevLocationAndCurrentCapital()
        {
            if (Pos > 0)
            {
                bool prevLocation = false;
                if (_prevWord.Length < 4)
                {
                    var previous = RemoveSymbols(RemoveApos(_prevWord));
                    if (previous.Length == 0)
                    {
                        prevLocation = T2.Equals("LOCATION");
                    }
                }

                if (T1.Equals("LOCATION"))
                {
                    if (char.IsUpper(Sentence[Pos][0]))
                    {
                        return "PREVLOCATIONCC:1:" + T;
                    }

                    // University of Washington
                    if (Sentence[Pos].Length < 4 && Pos < Sentence.Count -1 && char.IsUpper(Sentence[Pos+1][0]))
                    {
                        return "PREVLOCATIONCWORD:" + Sentence[Pos].Trim().ToLowerInvariant() + ":" + T;
                    }
                }
                
            }
            return null;
        }

        private string GetSalutationTag()
        {
            if (Pos > 0)
            {
                if (IsSalutationAbbr(Sentence[Pos - 1]))
                {
                    return "PrevWordSaltTAG:1:" + T;
                }
            }
            return null;
        }

        //private string GetTriGramFeature()
        //{
        //    return "TRIGRAMTAG:" + T2 +":"+ T1 +":"+ T;
        //}

        private string GetTagFeature()
        {
            // returning this feature for small words only to cover 
            if (_currentWord.Length < 4)
            {
                var input = _currentWord;
                if (input.EndsWith("'m"))
                {
                    input = input.Replace("'m", "");
                }
                input = RemoveApos(input);
                input = RemoveSymbols(input);
                
                return "TAG:" + input.ToLowerInvariant() + ":" + T;
            }
            return null;
        }

        private string GetCurrentWordPrevTag()
        {
            return "CurWPrevTAG:" + Sentence[Pos] + ":" + T1;
        }

        private string GetBiGram()
        {
            return "BIGRAMTAG:"+ T1 +":" + T;
        }

        private string GetSuffixTag(int suffix)
        {
            // sentence should be atleast 6 words and first word should be capital

            if (Pos >= Sentence.Count || ContainsApos(Sentence[Pos]) || Sentence[Pos].Length < 6
                || !char.IsUpper(Sentence[Pos][0])) return null;

            // next word should start with lower char
            if (Pos < Sentence.Count || (Sentence[Pos].Length > 2 && char.IsUpper(Sentence[Pos + 1][0])))
            {
                return null;
            }

            var input = Sentence[Pos];

            input = RemoveApos(input);
            input = RemoveSymbols(input);

            input = input.ToLowerInvariant();
            // remove s and es from end.
            if (input.EndsWith("es"))
            {
                input = input.Substring(0, input.Length - 2);
            }
            if (input.EndsWith("s"))
            {
                input = input.Substring(0, input.Length - 1);
            }

            var length = input.Length;
            if (length - suffix >= 0)
            {
                return "SUFF" + suffix.ToString(CultureInfo.InvariantCulture) +
                       "TAG:" + input.Substring(length - suffix) + ":" + T;
            }
            return null;
        }

        public string GetAllCapsTag()
        {
            if (Pos >= Sentence.Count)
            {
                return null;
            }
            if (IsAllUpper(Sentence[Pos]))
            {
                return "ALLUPPERTAG:1:" +T;
            }
            //return "ALLUPPERTAG:0:" + T;
            return null;
        }

        public string GetAlphaNumTag()
        {
            if (Pos >= Sentence.Count)
            {
                return null;
            }
            var alpha = false;
            var num = false;
            foreach (var ch in Sentence[Pos])
            {
                if (!alpha)
                {
                    alpha = char.IsLetter(ch);
                }
                if (!num)
                {
                    num = char.IsNumber(ch);
                }
                if (alpha && num)
                {
                    return "ALPHANUMTAG:1:" + T;
                }
            }
            //return "ALPHANUMTAG:O:" + T;
            return null;
        }

        public string GetStartWithCap()
        {
            if (Pos >= Sentence.Count)
            {
                return null;
            }
            if (_currentStartsWithCap && char.IsLetter(Sentence[Pos][0]) && !IsAllUpper(Sentence[Pos]))
            {
                if (Pos == 0 && Sentence.Count > 2 && Char.IsUpper(Sentence[Pos+1][0]))
                {
                    return "FIRSTCHARUPPERTAGATSTART:1:" + T;
                }
                else
                {
                    return "FIRSTCHARUPPERTAG:1:" + T;    
                }
            }
            //return "FIRSTCHARUPPERTAG:0:" + T;
            return null;
        }

        public string GetStartWithNum()
        {
            if (Pos >= Sentence.Count)
            {
                return null;
            }
            if (char.IsNumber(Sentence[Pos][0]))
            {
                return "FIRSTCHARNUMTAG:1:" + T;
            }
            //return "FIRSTCHARNUMTAG:0:" + T;
            return null;
        }

        public string GetNumTag()
        {
            if (Pos >= Sentence.Count)
            {
                return null;
            }
            var num = IsAllNum(Sentence[Pos]);
            if (num)
            {
                return "NUMTAG:1:" + T;
            }
            //return "NUMTAG:0:" + T;
            return null;
        }

        private bool IsAllNum(string input)
        {
            input = RemoveApos(input);
            input = RemoveSymbols(input);
            return input.All(char.IsNumber);
        }

        public string GetOneLength()
        {
            if (Pos >= Sentence.Count)
            {
                return null;
            }
            if (Sentence[Pos].Length == 1)
            {
                return "ONELENGTHTAG:1:" + T;
            }
            //return "ONELENGTHTAG:0:" + T;
            return null;
        }

        public string GetSymbolTag()
        {
            if (Pos > Sentence[Pos].Count())
            {
                return null;
            }
            if (IsAnySymbol(Sentence[Pos]))
            {
                return "ANYSYMBOLTAG:1:" + T;
            }
            //return "ANYSYMBOLTAG:0:" + T;
            return null;
        }

        private string GetPrevWordCurrentTag()
        {
            if (Pos > 0)
            {
                return "PREVWORDCURRTAG:" + _prevWord + ":" + T;
            }
            return null;
        }

        private string GetPrevOtherWordCurrentTag()
        {
            if (Pos > 1 && T1.Equals("OTHER"))
            {
                //if (T.Equals("LOCATION") ||
                //    (Sentence[Pos - 1].Length < 4))
                if (T.Equals("LOCATION") || _currentStartsWithCap)
                {
                    if (Config.Instance.VerbSet.Contains(_prevWord) || Config.Instance.ConjunctionSet.Contains(_prevWord))
                    {
                        // skip is,am, are and and of neither 
                        return "PREVOTHERWORDCURRTAG:" + Sentence[Pos-2].ToLowerInvariant().Trim() + ":" + T;
                    }
                    else
                    {
                        return "PREVOTHERWORDCURRTAG:" + _prevWord + ":" + T;    
                    }
                }
            }
            return null;
        }

        public string GetNextWordCurrentTag()
        {
            if (Pos >= Sentence.Count - 1)
                return null;
            return "NEXTWORDCURRTAG:" + _nextWord + ":" + T;
        }

        bool IsAnySymbol(string input)
        {
            //if (Sentence[Pos].Length > 2)
            //{
            //    char end = Sentence[Pos][Sentence[Pos].Length - 1];
            //    if (!end.Equals('.') && (char.IsSymbol(end) || char.IsPunctuation(end)))
            //        Sentence[Pos] = Sentence[Pos].Substring(0, Sentence[Pos].Length - 1);
            //}
            input = RemoveApos(input);
            input = RemoveSymbols(input);
            return input.Any(char.IsSymbol) || input.Any(char.IsPunctuation);
        }

        string RemoveSymbols(string input)
        {
            if (input.Length > 2)
            {
                char end = input[input.Length - 1];
                if ((char.IsSymbol(end) || char.IsPunctuation(end)))
                    input = input.Substring(0, input.Length - 1);
            }
            if (input.Length <= 0) return input;
            input = input.Replace("'", "");
            input = input.Replace("-", "");
            input = input.Replace("/", "");
            input = input.Replace("(", "");
            input = input.Replace(")", "");
            input = input.Replace(":", "");
            return input;
        }

        bool IsAllUpper(string input)
        {
            input = RemoveApos(input);
            input = RemoveSymbols(input);
            if (input.Length > 0)
                return input.All(t => Char.IsLetter(t) && Char.IsUpper(t));
            return false;
        }

        bool IsAllSmall(string input)
        {
            input = RemoveApos(input);
            input = RemoveSymbols(input);
            if (input.Length > 0)
            {
                return input.All(t => Char.IsLetter(t) && Char.IsLower(t));
            }
            // return true for empty string.
            return true;
        }

        public static float GetWeight(string feature)
        {
            if (feature.Contains("TRIGRAM:"))
            {
                return (float) .0001;
                //return (float)1;
            }
            else if (feature.Contains("TAG:"))
            {
                return 1;
            }
            return 1;
        }

        private bool IsSalutationAbbr(string word)
        {
            word = word.Trim();
            if (word.EndsWith(".") &&
                char.IsUpper(word[0]) &&
                word.Length <= 4)
            {
                if (word.Length >= 3)
                {
                    return char.IsLower(word[word.Length - 2]);
                }
            }
            return false;
        }

    }
}
