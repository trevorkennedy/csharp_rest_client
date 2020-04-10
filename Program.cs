using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Linq;

namespace RestClient
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private const string name = ".NET Repository Reporter";
        private const string layout = "{0,-30} {1,-5} {2:yyyy-MM-dd} {3,-40} ";
        private const string uri = "https://api.github.com/orgs/dotnet/repos";

        private static async Task ProcessRepositories()
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(name);
            Console.ForegroundColor = originalColor;
            
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", name);

            var serializer = new DataContractJsonSerializer(typeof(List<Repo>));
            var streamTask = client.GetStreamAsync(uri);
            var repositories = serializer.ReadObject(await streamTask) as List<Repo>;
            var orderByResult = from s in repositories
                                orderby s.StarCount descending
                                select s;

            Console.WriteLine(layout, "Name", "Stars", "Watchers", "Description");
            foreach (var repo in orderByResult)
                Console.WriteLine(layout, repo.Name, String.Format("\u001b[31m{0}\u001b[0m", repo.StarCount), repo.LastPush, repo.GitHubHomeUrl);
        }

        static void Main()
        {
            ProcessRepositories().Wait();
        }
    }
}
