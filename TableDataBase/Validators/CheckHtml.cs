using System.Text;
using Newtonsoft.Json;
using TableDataBase.Models;

namespace TableDataBaseMVC.Validators
{
	public static class CheckHtml
    {
        public static bool IsValid(string html)
        {
            if (!String.IsNullOrEmpty(html))
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)");
                        var content = new StringContent(html, Encoding.UTF8, "text/html");
                        var response = client.PostAsync("https://validator.w3.org/nu/?out=json", content).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var result =  response.Content.ReadAsStringAsync().Result;
                            var resultModel = JsonConvert.DeserializeObject<HtmlW3CValidatorModel>(result);
                            var errors = resultModel.Messages.Where(x => x.Type == "error").Select(x => x.Message);
                            if (errors.Count() > 0)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}

