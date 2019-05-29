using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;

namespace AzureDevOpsConnect.Controllers
{
    public class AzureDevOpsController : ApiController
    {
        public ActionResult Get()
        {
            try
            {
                var json = JsonConvert.SerializeObject(new { definition = new { id = "{Número da build a ser executada}" } });
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                // Obtem o token pessoal fornecido pelo AzureDevOps e que está no app.settings
                var personalaccesstoken = WebConfigurationManager.AppSettings["AzurePersonalToken"].Trim();

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                        Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", personalaccesstoken))));

                    using (HttpResponseMessage response = client.PostAsync("https://dev.azure.com/{Organization}/{Project}/_apis/build/builds?api-version=5.0", stringContent).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = response.Content.ReadAsStringAsync().Result;
                        Console.WriteLine(responseBody);
                    }
                }

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.ToString());
            }
        }
    }
}
