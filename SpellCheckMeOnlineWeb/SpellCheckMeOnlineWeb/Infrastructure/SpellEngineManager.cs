using NHunspell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace SpellCheckMeOnlineWeb.Infrastructure
{
    public static class SpellEngineManager
    {
        public static SpellEngine SpellEngine { get; set; }

        public static List<string> Languages { get; set; }

        public static Dictionary<string, List<char>> AlphabetLetters { get; set; }
        static SpellEngineManager()
        {
            try
            {
                Languages = new List<string>();
                AlphabetLetters = new Dictionary<string, List<char>>();

                string dictionaryPath = Hunspell.NativeDllPath;

                DirectoryInfo di = new DirectoryInfo(dictionaryPath);
                FileInfo[] files = di.GetFiles("*", SearchOption.AllDirectories);
                List<FileInfo> allAffFiles = files.Where(f => f.FullName.EndsWith(".aff", StringComparison.InvariantCultureIgnoreCase)).OrderBy(f => f.Name).ToList();
                List<FileInfo> allDicFiles = files.Where(f => f.FullName.EndsWith(".dic", StringComparison.InvariantCultureIgnoreCase)).OrderBy(f => f.Name).ToList();

                SpellEngine = new SpellEngine();

                foreach (FileInfo affFile in allAffFiles)
                {
                    string nameWithoutExtension = affFile.Name.Replace(".aff", string.Empty).ToLowerInvariant();
                  
                    LanguageConfig enConfig = new LanguageConfig();
                    enConfig.LanguageCode = nameWithoutExtension;
                    enConfig.HunspellAffFile = affFile.FullName;
                    enConfig.HunspellDictFile = affFile.FullName.Replace(".aff", ".dic");
                    enConfig.HunspellKey = "";
                    SpellEngine.AddLanguage(enConfig);
                    AlphabetLetters[nameWithoutExtension] = GetAlphabetLetters(enConfig.HunspellAffFile);
                    Languages.Add(nameWithoutExtension);
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

        private static List<char> GetAlphabetLetters(string filePathOfAffFile)
        {
            string enc = File.ReadAllLines(filePathOfAffFile).FirstOrDefault().Substring(4);
            Encoding encoding = Encoding.GetEncoding(enc);

            var allLines = File.ReadAllLines(filePathOfAffFile.Replace(".aff", ".dic"), encoding);
            StringBuilder sb = new StringBuilder();
            bool isFirstLine = true;
            foreach (string line in allLines)
            {
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }
                int indexOfSlash = line.IndexOf("/");
                if (indexOfSlash == -1)
                {
                    sb.Append(line);
                }
                else
                {
                    var textOnly = line.Substring(0, indexOfSlash);
                    sb.Append(textOnly);
                }            
                if (line.Contains("Å"))
                {

                }
            }
            var result = sb.ToString().ToCharArray().Distinct().OrderBy(c => c).ToList(); // .Where(c => char.IsLetter(c)
            return result;
        }
    }
}