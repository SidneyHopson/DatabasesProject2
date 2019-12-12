using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarkLogic.Controllers
{
    public class MarkLogicController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<string> Get()
        {
            using (var client = new HttpClient())
            {
                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential("wenli", "WANG3838"),
                };
                var httpClient = new HttpClient(httpClientHandler);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var result = await httpClient.GetStringAsync("http://localhost:8000/LATEST/documents?uri=/example/test.json");
                return result;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upload( List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    Stream s = formFile.OpenReadStream();
                    HttpContent content = new StreamContent(s);
                    using (var client = new HttpClient())
                    {
                        var httpClientHandler = new HttpClientHandler()
                        {
                            Credentials = new NetworkCredential("wenli", "WANG3838"),
                        };
                        var httpClient = new HttpClient(httpClientHandler);
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        await httpClient.PutAsync("http://localhost:8000/LATEST/documents?uri=/example/test.json", content);
                    }
                }
            }
            return Ok(new { count = files.Count, size});
            // NuGet Newtonsoft.Json library for object conversion
            // string json = JsonConvert.SerializeObject(account, Formatting.Indented);
            // Account account_back = JsonConvert.DeserializeObject<Account>(json);
        }
    }
}