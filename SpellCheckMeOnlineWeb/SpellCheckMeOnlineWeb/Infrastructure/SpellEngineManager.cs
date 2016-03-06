using NHunspell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SpellCheckMeOnlineWeb.Infrastructure
{
    public static class SpellEngineManager
    {
        public static SpellEngine SpellEngine { get; set; }
        static SpellEngineManager()
        {
            try
            {
                string dictionaryPath = Hunspell.NativeDllPath;

                DirectoryInfo di = new DirectoryInfo(dictionaryPath);
                FileInfo[] files = di.GetFiles("*", SearchOption.AllDirectories);

                SpellEngine = new SpellEngine();
                LanguageConfig enConfig = new LanguageConfig();
                enConfig.LanguageCode = "en";
                enConfig.HunspellAffFile = files.FirstOrDefault(f => f.FullName.EndsWith("en_us.aff", StringComparison.InvariantCultureIgnoreCase)).FullName; //Path.Combine(dictionaryPath, "en_us.aff");
                enConfig.HunspellDictFile = files.FirstOrDefault(f => f.FullName.EndsWith("en_us.dic", StringComparison.InvariantCultureIgnoreCase)).FullName; //Path.Combine(dictionaryPath, "en_us.dic");
                enConfig.HunspellKey = "";
                SpellEngine.AddLanguage(enConfig);

                enConfig = new LanguageConfig();
                enConfig.LanguageCode = "bg";
                enConfig.HunspellAffFile = files.FirstOrDefault(f => f.FullName.EndsWith("bg.aff", StringComparison.InvariantCultureIgnoreCase)).FullName; //Path.Combine(dictionaryPath, "en_us.aff");
                enConfig.HunspellDictFile = files.FirstOrDefault(f => f.FullName.EndsWith("bg.dic", StringComparison.InvariantCultureIgnoreCase)).FullName; //Path.Combine(dictionaryPath, "en_us.dic");
                enConfig.HunspellKey = "";
                SpellEngine.AddLanguage(enConfig);


            }
            catch (Exception)
            {
                if (SpellEngine != null)
                    SpellEngine.Dispose();

                throw;
            }
        }
    }
}