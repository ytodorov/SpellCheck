using SpellCheckMeOnlineWeb.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SpellCheckMeOnlineWeb.Controllers
{
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

        public ActionResult GetHtml(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string html = httpClient.GetStringAsync(url).Result;

                //?
                if (!url.EndsWith("/"))
                {
                    url += "/";
                }
                html = html.Replace("href=\"/", "href=\"" + url);
                html = html.Replace("src=\"/", "src=\"" + url);

                string urlBase = new Uri(url).OriginalString;

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

                html = html.Insert(indexOfHead + 1, "<base href='" + urlBase + "'>");
                return Json(html, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult SpellText(string htmlEncoded, string text, string langugage)
        {
            string html = Server.HtmlDecode(htmlEncoded);

            var chars = text.ToCharArray();
            List<string> emptyChars = new List<string>();
            foreach (char c in chars)
            {
                if (string.IsNullOrWhiteSpace(c.ToString()))
                {
                    emptyChars.Add(c.ToString());
                }
            }

            var words = text.Replace("\n", " ").Replace(",", " ").Replace(".", " ").Split(emptyChars.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> wrongWords = new List<string>();
            words = words.Select(w => w.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).Where(w => ContainsLetters(w)).ToList();
            foreach (string word in words)
            {
                var res = SpellEngineManager.SpellEngine[langugage].Spell(word);
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
                var suggest = SpellEngineManager.SpellEngine[langugage].Suggest(word);
                StringBuilder sb = new StringBuilder();
                foreach (var item in suggest)
                {
                    sb.Append(item + ", ");
                   
                }               
                string suggestions = sb.ToString();
                sbSummary.Append(CreateGridRow(word, suggestions));

               

                html = html.Replace(word, $"<span title='{suggestions}' style='color:blueviolet;background-color:yellow'>{word}</span>");
            }

            sbSummary.Append("</tbody></table>");

            var returnObject = new { html = html + "<div></div>", summary = sbSummary.ToString() };
            return Json(returnObject);
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

        string CreateGridRow(string word, string suggestion)
        {
            string row = $@"<tr>
                        <td>{word}</td>
                        <td>{suggestion}</td>
                            </tr>";
            return row;
                        
        }
    }
}