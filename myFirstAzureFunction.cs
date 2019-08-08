using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;

namespace azureFunctionsWithDocker
{
    public static class myFirstAzureFunction
    {
        [FunctionName("myFirstAzureFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request. Translates any text to english");

            string sourceText = req.Query["sourceText"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            sourceText = sourceText ?? data?.sourceText;
            string APIkey = "Get a free API key at https://translate.yandex.com/developers/keys";

            string uri = "https://translate.yandex.net/api/v1.5/tr.json/translate?&key=" + key + "&lang=en&text=" + sourceText;
            HttpClient client = new HttpClient();
            string responseBody = await client.GetStringAsync(uri);
            dynamic responseBodySerialized = JsonConvert.DeserializeObject(responseBody);
            string targetText = responseBodySerialized?.text[0];

            return sourceText != null && targetText != null
                ? (ActionResult)new OkObjectResult("\"" + sourceText + "\"" + " translated to English: " + targetText + "\n \n \nPowered by Yandex http://translate.yandex.com")
                : new BadRequestObjectResult("Please pass a text on the query string or in the request body");
        }
    }
}
