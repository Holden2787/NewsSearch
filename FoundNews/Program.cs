using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using NewsSearch.Conf;

namespace NewsSearch
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var cnfg = Configuration.Settings;

            if(cnfg == null || string.IsNullOrEmpty(cnfg.ApiKey))
            {
                Console.Write("Конфигурация в файле настроек Settings.xml не задана.");
                Console.ReadLine();
                return;
            }

            Program program = new Program();

            await program.GetArticles(cnfg);
        }

        private async Task GetArticles(Settings cnfg) 
        {

           Console.Write($"Введите тему - ");
           var topic = Console.ReadLine();

            HttpClient client = new HttpClient();

            var url = $"https://newsapi.org/v2/everything?q={topic}&searchIn=title&from=2023-09-02&sortBy=popularity&apiKey={cnfg.ApiKey}";

            string response = await client.GetStringAsync(url);

            NewResponse  newsObject = JsonConvert.DeserializeObject<NewResponse>(response);
            var k = 1;
            foreach (var article in newsObject.Articles)
            {
               //Console.WriteLine(article.Authot);
                Console.WriteLine($"{k}: {article.Title}");
                //Console.WriteLine(article.Description);
                //Console.WriteLine(article.Content);
                Console.WriteLine();
                k++;
            }
            Console.Write("ВВЕДИТЕ НОМЕР НОВОСТИ КОТОРУЮ ХОТИТЕ ПРОАНАЛИЗИРОВАТЬ - ");
            var index = Convert.ToInt32(Console.ReadLine()) - 1;

            while (index+1 > newsObject.Articles.Count || index+1 <= 0)
            {
                Console.Write("Такого номера новости не существует в опубликованном списке. Повторите ввод - ");
                index = Convert.ToInt32(Console.ReadLine()) - 1;
            
            
            }
            for (var i = 0; i < newsObject.Articles.Count; i++) 
            {
                if (i == index)
                {
                    Console.WriteLine(newsObject.Articles[i].Title);
                    Console.WriteLine();
                    int cnt = 0;
                    var result = DetectVowels(newsObject.Articles[i].Title, ref cnt);
                    Console.WriteLine($"Слово с наибольшим количеством гласных {{{cnt}}} - {result}");
                }
            }

            Console.ReadLine();
            client.Dispose();
        }

        private string DetectVowels(string descr, ref int mostVowels)
        {
            char[] sk = { ' ', '.', ',', '!', '?', ':', ';', '(', ')', '\t' };
            string[] parts = descr.Split(sk, StringSplitOptions.RemoveEmptyEntries);

            var r = new Regex("(a|e|i|o|u|y|A|E|I|O|U|Y|а|у|о|ы|и|э|я|ю|ё|е|А|У|О|Ы|И|Э|Я|Ю|Ё|Е)");

            mostVowels = parts.Max(y => r.Matches(y).Count);
            var cnt = mostVowels;
            return string.Join(", ", parts.Where(x => r.Matches(x).Count == cnt));
        }
    }

    class NewResponse
    {
        public  string status {get; set;}
        public int TotalResult { get; set;}
        public List<Article> Articles { get; set;}
    }

    class Article
    {
        public string Author { get; set;}
        public string Title { get; set; }
        public string Description { get; set;}
        public string Content { get; set;}
    }
}
