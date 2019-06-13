using AzureDevOps.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AzureDevOpsConnectNetCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AzureDevOpsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AzureDevOpsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Queue")]
        public IActionResult Get(DevOpsParameters data)
        {
            try
            {
                // Informações sobre o pipeline de execução
                var personalaccesstoken = _configuration.GetSection("PersonalToken").Value;
                var organizationDevOps = _configuration.GetSection("OrganizationDevOps").Value;
                var projectDevOps = _configuration.GetSection("ProjectDevOps").Value;
                var buildNumber = _configuration.GetSection("BuildNumber").Value;

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

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
