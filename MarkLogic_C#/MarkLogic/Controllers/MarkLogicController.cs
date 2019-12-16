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
        public async Task<IActionResult> Browse(string start)
        {
            start = "0";
            string pageLength = "100";
            using (var client = new HttpClient())
            {
                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential("admin", "admin"),
                };
                var httpClient = new HttpClient(httpClientHandler);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                var result = await httpClient.GetStreamAsync("http://e8instructsql2.ad.psu.edu:8000/LATEST/search?start=" + start + "&pageLength=" + pageLength);
                return View(result);
            }
            
        }

       

        public async Task<string> Get(string uri)
        {
            using (var client = new HttpClient())
            {
                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential("admin", "admin"),
                };
                var httpClient = new HttpClient(httpClientHandler);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                var result = await httpClient.GetStringAsync("http://e8instructsql2.ad.psu.edu:8000/LATEST/documents?uri=" + uri);
                return result;
            }
        }

        public IActionResult Upload()
        {
            return View();
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
                            Credentials = new NetworkCredential("admin", "admin"),
                        };
                        var httpClient = new HttpClient(httpClientHandler);
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(formFile.ContentType));
                        await httpClient.PutAsync("http://e8instructsql2.ad.psu.edu:8000/LATEST/documents?uri=/sdh5404/" + formFile.FileName, content);
                    }
                }
                
            }
            return RedirectToAction(nameof(Browse));
            //return Ok(new { count = files.Count, size});
            // NuGet Newtonsoft.Json library for object conversion
            // string json = JsonConvert.SerializeObject(account, Formatting.Indented);
            // Account account_back = JsonConvert.DeserializeObject<Account>(json);
        }

        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string uri)
        {
                using (var client = new HttpClient())
                {
                    var httpClientHandler = new HttpClientHandler()
                    {
                        Credentials = new NetworkCredential("admin", "admin"),
                    };
                    var httpClient = new HttpClient();
                    await httpClient.DeleteAsync("http://e8instructsql2.ad.psu.edu:8000/LATEST/documents?uri=" + uri);
                }

            return RedirectToAction(nameof(Browse));
        }

        public IActionResult Download()
        {
            return View();
        }

        public async Task<FileContentResult> GetDoc(string uri)
        {
            using (var client = new HttpClient())
            {
                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential("admin", "admin"),
                };
                var httpClient = new HttpClient(httpClientHandler);
                var result = await httpClient.GetStreamAsync("http://e8instructsql2.ad.psu.edu:8000/LATEST/documents?uri=" + uri);

                HttpContent content = new StreamContent(result);
                byte[] data = await content.ReadAsByteArrayAsync();

                int index = uri.LastIndexOf("/");
                string fileName = uri.Substring(index);
                Response.Headers.Add("Content-Disposition", "filename=" + fileName);
                return File(data, "application/xml");
            }
        }

        public IActionResult SimpleSearch()
        {
            return View();
        }

        public async Task<Stream> StringQueryDoc(string uri, string start)
        {
            start = "0";
            string length = "100";
            using (var client = new HttpClient())
            {
                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential("admin", "admin"),
                };
                var httpClient = new HttpClient(httpClientHandler);
                var result = await httpClient.GetStreamAsync("http://e8instructsql2.ad.psu.edu:8000/LATEST/search?q=" + uri +"&start=" + start +"&pageLength=" + length);
                StreamContent content = new StreamContent(result);
                
                
                // Retrieving the confidence and fitness levels through the received XML document.
                //doc.Confidence = Convert.ToDouble(node.Attributes["confidence"].InnerXml);
                //doc.Fitness = Convert.ToDouble(node.Attributes["fitness"].InnerXml);

                return result;
            }
        }

        public async Task<Stream> StructuredQueryDoc(string uri, string start)
        {
            string length = "100";
            using (var client = new HttpClient())
            {
                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential("admin", "admin"),
                };
                var httpClient = new HttpClient(httpClientHandler);
                var result = await httpClient.GetStreamAsync("http://e8instructsql2.ad.psu.edu:8000/LATEST/search?structuredQuery=" + uri + "&start=" + start + "&pageLength=" + length);


                // Retrieving the confidence and fitness levels through the received XML document.
                //doc.Confidence = Convert.ToDouble(node.Attributes["confidence"].InnerXml);
                //doc.Fitness = Convert.ToDouble(node.Attributes["fitness"].InnerXml);

                return result;
            }
        }


        public async Task<IActionResult> QBEQueryDoc(string uri, string length)
        {
            using (var client = new HttpClient())
            {
                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential("admin", "admin"),
                };
                var httpClient = new HttpClient(httpClientHandler);
                var result = await httpClient.GetAsync("http://e8instructsql2.ad.psu.edu:8000/LATEST/qbe?query=" + uri + "&pageLength=" + length);


                // Retrieving the confidence and fitness levels through the received XML document.
                //doc.Confidence = Convert.ToDouble(node.Attributes["confidence"].InnerXml);
                //doc.Fitness = Convert.ToDouble(node.Attributes["fitness"].InnerXml);

                return View(result);
            }
            
        }
    }
}