using HtmlAgilityPack;
using NHunspell;
using SpellCheckMeOnlineWeb.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SpellCheckMeOnlineWeb.Controllers
{
    [ValidateInput(false)]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Index2()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult GetHtml(string urlString)
        {


            if (string.IsNullOrEmpty(urlString))
            {
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }

            if (!urlString.EndsWith("/"))
            {
                urlString += "/";
            }

            Uri url = null;

            try
            {
                url = new Uri(urlString);
            }
            catch (FormatException ex)
            {
                return Json(ex.Message);
            }

            string baseString = urlString;
            if (url.LocalPath != "/")
            {
                baseString = url.OriginalString.Replace(url.LocalPath, string.Empty) + "/";
            }


            using (HttpClient httpClient = new HttpClient())
            {
                string html = httpClient.GetStringAsync(urlString).Result;

                //?
                if (!urlString.EndsWith("/"))
                {
                    urlString += "/";
                }
                html = html.Replace("href=\"/", "href=\"" + baseString);
                html = html.Replace("src=\"/", "src=\"" + baseString);

                var indexOfHead = html.IndexOf("<head");
                for (int i = indexOfHead; i < html.Length; i++)
                {
                    char currentChar = html[i];
                    if (currentChar.Equals('>'))
                    {
                        indexOfHead = i;
                        break;
                    }
                }

                html = html.Insert(indexOfHead + 1, "<base href='" + baseString + "'>");
                return Json(html, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult SpellText(string htmlEncoded, string text, string langugage)
        {
            FileInfo affFile = SpellEngineManager.AllAffFiles.FirstOrDefault(f => f.Directory.Name.Equals(langugage, StringComparison.InvariantCultureIgnoreCase));
            string nameWithoutExtension = affFile.Directory.Name.ToLower();// affFile.Name.Replace(".aff", string.Empty).ToLowerInvariant();

            using (SpellEngine spellEngine = new SpellEngine())
            {
                LanguageConfig enConfig = new LanguageConfig();
                enConfig.LanguageCode = nameWithoutExtension;
                enConfig.HunspellAffFile = affFile.FullName;
                enConfig.HunspellDictFile = affFile.FullName.Replace(".aff", ".dic");
                enConfig.HunspellKey = "";
                spellEngine.AddLanguage(enConfig);
                var alphabetCharsOld = SpellEngineManager.GetAlphabetLetters(enConfig.HunspellAffFile);
                SpellEngineManager.AlphabetLetters[nameWithoutExtension] = alphabetCharsOld;






                string html = Server.HtmlDecode(htmlEncoded);

                //HtmlDocument doc = new HtmlDocument();
                //doc.LoadHtml(html);
                ////doc.Save(Console.Out); // show before 
                //RemoveComments(doc.DocumentNode);
                //var test = doc.DocumentNode.InnerHtml;
                              


                //?? test
                //var indexOfBase = html.IndexOf("<base");
                //if (indexOfBase > 0)
                //{
                //    html = html.Substring(indexOfBase);
                //    html = html.Replace("</body>", string.Empty);
                //    html = html.Replace("</html>", string.Empty);
                   
                //}


                var testForClearText = StripTagsCharArray(html);
                // ?? test
                text = testForClearText;

                var chars = text.ToCharArray();
                List<string> emptyChars = new List<string>();
                foreach (char c in chars)
                {
                    if (string.IsNullOrWhiteSpace(c.ToString()))
                    {
                        emptyChars.Add(c.ToString());
                    }
                }
                emptyChars = emptyChars.Distinct().ToList();

                var alphabetChars = SpellEngineManager.AlphabetLetters[langugage];

                //var punctuationChars = chars.Where(c => char.IsPunctuation(c)).Distinct().ToList();
                var nonAlphabetChars = chars.Distinct().Except(alphabetChars).ToList();
                //var nonAlphabetChars = chars.Distinct().Except(alphabetChars).Except(punctuationChars).ToList();

                //for (int i = 0; i < punctuationChars.Count; i++)
                //{
                //    text = text.Replace(punctuationChars[i].ToString(), " ");
                //}

                //var words = text.Replace("\n", " ").Replace(",", " ").Replace(".", " ").Split(emptyChars.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                var words = text.Split(emptyChars.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();



                //var words = text.Replace("\n", " ").Replace(",", " ").Replace(".", " ").Split(emptyChars.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                List<string> wrongWords = new List<string>();

                //words = words.Select(w => w.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).Where(w => ContainsLetters(w)).ToList();
                words = words.Select(w => w.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).Where(w => ContainsOnlyAlphabetLetters(w, alphabetChars)).ToList();

                foreach (string word in words)
                {
                    var res = spellEngine[langugage].Spell(word);
                    if (!res)
                    {
                        wrongWords.Add(word);
                    }
                }
                wrongWords = wrongWords.Distinct().ToList();

                StringBuilder sbSummary = new StringBuilder();
                string begin = @"  <table id='grid'>
                <colgroup>
                    <col />
                    <col />                    
                </colgroup>
                <thead>
                    <tr>
                        <th data-field='make'>Word</th>
                        <th data-field='model'>Suggestion</th>                        
                    </tr>
                </thead>
                <tbody>";
                sbSummary.Append(begin);

                foreach (string word in wrongWords)
                {
                    var suggest = spellEngine[langugage].Suggest(word);
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in suggest)
                    {
                        sb.Append(item + ", ");

                    }
                    string suggestions = sb.ToString();


                    //var wordsInHtml = html.Split(emptyChars.ToArray(), StringSplitOptions.None);
                    //StringBuilder newHtml = new StringBuilder();
                    //for (int i = 0; i < wordsInHtml.Count(); i++)
                    //{
                    //    var currWord = wordsInHtml[i];
                    //    if (currWord.Trim() == word.Trim())
                    //    {
                    //        newHtml.Append($"<span title='{suggestions}' style='color:blueviolet;background-color:yellow'>{word}</span>");
                    //    }
                    //    else
                    //    {
                    //        newHtml.Append(currWord);
                    //    }
                    //}

                    //html = newHtml.ToString();

                    // заради Angstriom го правя това - съдържа не само букви
                    //if (word.ToCharArray().All(c => char.IsLetter(c)))
                    {

                        string input = html;
                        string pattern = $@"\b{word}\b";
                        string replace = $"<span title='{suggestions}' style='color:blueviolet;background-color:yellow'>{word}</span>";
                        var newhtml = Regex.Replace(input, pattern, replace);
                        if (newhtml != html)
                        {
                            // за да няма в грида повече думи отколкото сме подчертали в editor
                            sbSummary.Append(CreateGridRow(word, suggestions));
                        }
                        html = newhtml;
                    }


                    //html = html.Replace(word, $"<span title='{suggestions}' style='color:blueviolet;background-color:yellow'>{word}</span>");
                }

                sbSummary.Append("</tbody></table>");

                var returnObject = new { html = html + "<div></div>", summary = sbSummary.ToString() };
                return Json(returnObject);
            }
        }

        bool ContainsLetters(string wordToCheck)
        {
            var arr = wordToCheck.ToCharArray();
            foreach (char c in arr)
            {
                if (char.IsLetter(c))
                {
                    return true;
                }
            }
            return false;
        }

        bool ContainsOnlyAlphabetLetters(string wordToCheck, List<char> alphabetLetters)
        {
            List<char> wordChars = wordToCheck.ToCharArray().ToList();

            if (alphabetLetters.Union(wordChars).Count() != alphabetLetters.Count)
            {
                return false;
            }

            return true;
        }

        string CreateGridRow(string word, string suggestion)
        {
            string row = $@"<tr>
                        <td>{word}</td>
                        <td>{suggestion}</td>
                            </tr>";
            return row;

        }

        static void RemoveComments(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Comment)
            {
                node.ParentNode.RemoveChild(node);
                return;
            }

            if (!node.HasChildNodes)
                return;

            foreach (HtmlNode subNode in node.ChildNodes)
            {
                RemoveComments(subNode);
            }
        }

        public static string StripTagsCharArray(string source)
        {
            try
            {
                string result;

                // Remove HTML Development formatting
                // Replace line breaks with space
                // because browsers inserts space
                result = source.Replace("\r", " ");
                // Replace line breaks with space
                // because browsers inserts space
                result = result.Replace("\n", " ");
                // Remove step-formatting
                result = result.Replace("\t", string.Empty);
                // Remove repeating spaces because browsers ignore them
                result = System.Text.RegularExpressions.Regex.Replace(result,
                                                                      @"( )+", " ");

                // Remove the header (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*head([^>])*>", "<head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*head( )*>)", "</head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<head>).*(</head>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all scripts (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*script([^>])*>", "<script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*script( )*>)", "</script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"(<script>)([^(<script>\.</script>)])*(</script>)",
                //         string.Empty,
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<script>).*(</script>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all styles (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*style([^>])*>", "<style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*style( )*>)", "</style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<style>).*(</style>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert tabs in spaces of <td> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*td([^>])*>", "\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line breaks in places of <BR> and <LI> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*br( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*li( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line paragraphs (double line breaks) in place
                // if <P>, <DIV> and <TR> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*div([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*tr([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*p([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // Remove remaining tags like <a>, links, images,
                // comments etc - anything that's enclosed inside < >
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<[^>]*>", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // replace special characters:
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @" ", " ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&bull;", " * ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lsaquo;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&rsaquo;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&trade;", "(tm)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&frasl;", "/",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lt;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&gt;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&copy;", "(c)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&reg;", "(r)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove all others. More can be added, see
                // http://hotwired.lycos.com/webmonkey/reference/special_characters/
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&(.{2,6});", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // for testing
                //System.Text.RegularExpressions.Regex.Replace(result,
                //       this.txtRegex.Text,string.Empty,
                //       System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // make line breaking consistent
                result = result.Replace("\n", "\r");

                // Remove extra line breaks and tabs:
                // replace over 2 breaks with 2 and over 4 tabs with 4.
                // Prepare first to remove any whitespaces in between
                // the escaped characters and remove redundant tabs in between line breaks
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\t)", "\t\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\r)", "\t\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\t)", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove redundant tabs
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove multiple tabs following a line break with just one tab
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Initial replacement target string for line breaks
                string breaks = "\r\r\r";
                // Initial replacement target string for tabs
                string tabs = "\t\t\t\t\t";
                for (int index = 0; index < result.Length; index++)
                {
                    result = result.Replace(breaks, "\r\r");
                    result = result.Replace(tabs, "\t\t\t\t");
                    breaks = breaks + "\r";
                    tabs = tabs + "\t";
                }

                // That's it.
                return result;
            }
            catch (Exception ex)
            {
                throw;
                //log.Error(ex);
                //return source;
            }
        }
    }
}