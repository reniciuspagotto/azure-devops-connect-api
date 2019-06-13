using AzureDevOps.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        // POST api/azuredevops
        public ActionResult Post([FromBody] DevOpsParameters data)
        {
            try
            {
                // Informações sobre o pipeline de execução
                var personalaccesstoken = WebConfigurationManager.AppSettings["PersonalToken"].Trim();
                var organizationDevOps = WebConfigurationManager.AppSettings["OrganizationDevOps"].Trim();
                var projectDevOps = WebConfigurationManager.AppSettings["ProjectDevOps"].Trim();
                var buildNumber = WebConfigurationManager.AppSettings["BuildNumber"].Trim();

                var parameters = new Dictionary<string, string>
                {
                    { "projectname", data.ProjectName }
                };

                var appRequest = new DevOpsInfo
                {
                    Definition = new BuildDefinition(128),
                    Parameters = JsonConvert.SerializeObject(parameters),
                };

                var stringContent = new StringContent(JsonConvert.SerializeObject(appRequest), Encoding.UTF8, "application/json");

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                        Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", personalaccesstoken))));

                    using (HttpResponseMessage response = client.PostAsync($"https://dev.azure.com/{organizationDevOps}/{projectDevOps}/_apis/build/builds?api-version=5.0", stringContent).Result)
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