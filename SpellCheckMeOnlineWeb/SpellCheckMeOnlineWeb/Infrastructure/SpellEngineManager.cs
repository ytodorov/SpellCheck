using NHunspell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SpellCheckMeOnlineWeb.Infrastructure
{
    public static class SpellEngineManager
    {
        private static SpellEngine SpellEngine { get; set; }

        public static List<string> Languages { get; set; }

        public static List<FileInfo> AllAffFiles { get; set; }

        public static List<FileInfo> AllDicFiles { get; set; }

        public static Dictionary<string, List<char>> AlphabetLetters { get; set; }
        static SpellEngineManager()
        {
            try
            {
                Languages = new List<string>();
                AlphabetLetters = new Dictionary<string, List<char>>();

                string dictionaryPath = new DirectoryInfo(Hunspell.NativeDllPath).Parent.FullName + "\\dicts";

                DirectoryInfo di = new DirectoryInfo(dictionaryPath);
                FileInfo[] files = di.GetFiles("*", SearchOption.AllDirectories);
                List<FileInfo> allAffFiles = files.Where(f => f.FullName.EndsWith(".aff", StringComparison.InvariantCultureIgnoreCase)).OrderBy(f => f.Name).ToList();
                AllAffFiles = allAffFiles;
                List<FileInfo> allDicFiles = files.Where(f => f.FullName.EndsWith(".dic", StringComparison.InvariantCultureIgnoreCase)).OrderBy(f => f.Name).ToList();
                AllDicFiles = allDicFiles;


                SpellEngine = new SpellEngine();

                foreach (FileInfo affFile in allAffFiles)
                {
                    try
                    {
                        //affFile.Directory.Parent.Name

                        string nameWithoutExtension = affFile.Directory.Name.ToLower();// affFile.Name.Replace(".aff", string.Empty).ToLowerInvariant();

                        LanguageConfig enConfig = new LanguageConfig();
                        enConfig.LanguageCode = nameWithoutExtension;
                        enConfig.HunspellAffFile = affFile.FullName;
                        enConfig.HunspellDictFile = affFile.FullName.Replace(".aff", ".dic");
                        enConfig.HunspellKey = "";
                        //SpellEngine.AddLanguage(enConfig);
                        //var alphabetChars = GetAlphabetLetters(enConfig.HunspellAffFile);
                        //AlphabetLetters[nameWithoutExtension] = alphabetChars;
                        Languages.Add(nameWithoutExtension);

                    }
                    catch (Exception ex)
                    {

                    }
                }








                //SpellEngine = new SpellEngine();
                //LanguageConfig enConfig = new LanguageConfig();
                //enConfig.LanguageCode = "en";
                //enConfig.HunspellAffFile = files.FirstOrDefault(f => f.FullName.EndsWith("en_us.aff", StringComparison.InvariantCultureIgnoreCase)).FullName; //Path.Combine(dictionaryPath, "en_us.aff");
                //enConfig.HunspellDictFile = files.FirstOrDefault(f => f.FullName.EndsWith("en_us.dic", StringComparison.InvariantCultureIgnoreCase)).FullName; //Path.Combine(dictionaryPath, "en_us.dic");
                //enConfig.HunspellKey = "";
                //SpellEngine.AddLanguage(enConfig);
                //AlphabetLetters["en"] = GetAlphabetLetters(enConfig.HunspellAffFile);


                //enConfig = new LanguageConfig();
                //enConfig.LanguageCode = "bg";
                //enConfig.HunspellAffFile = files.FirstOrDefault(f => f.FullName.EndsWith("bg.aff", StringComparison.InvariantCultureIgnoreCase)).FullName; //Path.Combine(dictionaryPath, "en_us.aff");
                //enConfig.HunspellDictFile = files.FirstOrDefault(f => f.FullName.EndsWith("bg.dic", StringComparison.InvariantCultureIgnoreCase)).FullName; //Path.Combine(dictionaryPath, "en_us.dic");
                //enConfig.HunspellKey = "";
                //SpellEngine.AddLanguage(enConfig);
                //AlphabetLetters["bg"] = GetAlphabetLetters(enConfig.HunspellAffFile);


            }
            catch (Exception)
            {
                if (SpellEngine != null)
                    SpellEngine.Dispose();

                throw;
            }
        }

        public static List<char> GetAlphabetLetters(string filePathOfAffFile)
        {
            //return new List<char>();
            List<char> uniqueChars = new List<char>();

            var allEncodings = Encoding.GetEncodings().OrderBy(e => e.DisplayName);
            var namesEncodings = Encoding.GetEncodings().OrderBy(e => e.Name).ToList().Select( s => s.Name).ToList();
            var displaynamesEncodings = Encoding.GetEncodings().OrderBy(e => e.DisplayName).ToList().Select(s => s.DisplayName).ToList();



            string enc = File.ReadAllLines(filePathOfAffFile).FirstOrDefault(f => f.StartsWith("SET")).Substring(4);
            Encoding encoding = null;
            if (enc.Trim().Equals(Encoding.UTF8.BodyName.ToUpper().Trim()))
            {
                encoding = Encoding.UTF8;
            }
            else
            {
                if (enc.StartsWith("CP"))
                {
                    string codePage = enc.Replace("CP", string.Empty);
                    int cp = int.Parse(codePage);
                    encoding = allEncodings.FirstOrDefault(e => e.CodePage.ToString() == codePage).GetEncoding();
                }
                else if (enc.StartsWith("ISO"))
                {
                    encoding = allEncodings.FirstOrDefault(e => e.Name.Equals(enc.Replace("ISO", "ISO-"), StringComparison.InvariantCultureIgnoreCase)).GetEncoding();
                }
                else
                {
                    encoding = Encoding.GetEncoding(enc);
                }

            }


            var allLines = File.ReadAllLines(filePathOfAffFile.Replace(".aff", ".dic"), encoding);

            foreach (string line in allLines)
            {
                uniqueChars = uniqueChars.Distinct().ToList();
                uniqueChars.AddRange(line.ToCharArray().Distinct());
            }
            return uniqueChars.Distinct().ToList();

            //StringBuilder sb = new StringBuilder();
            //bool isFirstLine = true;
            //foreach (string line in allLines)
            //{
            //    if (isFirstLine)
            //    {
            //        isFirstLine = false;
            //        continue;
            //    }
            //    int indexOfSlash = line.IndexOf("/");
            //    if (indexOfSlash == -1)
            //    {
            //        sb.Append(line);
            //    }
            //    else
            //    {
            //        var textOnly = line.Substring(0, indexOfSlash);
            //        sb.Append(textOnly);
            //    }
            //    if (line.Contains("Å"))
            //    {

            //    }
            //}
            //var result = sb.ToString().ToCharArray().Distinct().OrderBy(c => c).ToList(); // .Where(c => char.IsLetter(c)
            //GC.Collect();
           // GC.WaitForPendingFinalizers();
            //return result;
        }
    }
}