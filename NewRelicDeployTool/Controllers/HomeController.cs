using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace NewRelicDeployTool.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Deploy(string key, string appName, string appID, string notes, string revision, string changes, string user)
        {
            bool success = false;
            string message = string.Empty;

            if (!string.IsNullOrEmpty(key))
            {
                if (!string.IsNullOrEmpty(appName) || !string.IsNullOrEmpty(appID))
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("x-api-key", key);

                        Dictionary<string, string> data = new Dictionary<string, string>();
                        data.Add("deployment[app_name]", appName);
                        data.Add("deployment[application_id]", appID);
                        data.Add("deployment[description]", notes);
                        data.Add("deployment[revision]", revision);
                        data.Add("deployment[changelog]", changes);
                        data.Add("deployment[user]", user);

                        var requestTask = client.PostAsync("https://api.newrelic.com/deployments.xml", new StringContent(JsonConvert.SerializeObject(data)));
                        
                        HttpResponseMessage response = requestTask.Result;

                        if(response.StatusCode == HttpStatusCode.OK)
                        {
                            success = true;
                        }
                        else
                        {
                            message = response.ReasonPhrase;
                        }
                    }
                }
                else
                    message = "App name or ID are required";
            }
            else
                message = "API Key is required";


            return Json(new { Success = success, Message = message });
        }
    }
}