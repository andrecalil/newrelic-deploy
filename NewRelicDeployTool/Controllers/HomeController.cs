using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
                        HttpRequestMessage post = new HttpRequestMessage(HttpMethod.Post, "https://api.newrelic.com/deployments.xml");
                        post.Headers.Add("x-api-key", key);

                        StringBuilder rawContent = new StringBuilder();
                        rawContent.AppendFormat("deployment[app_name]={0}&", appName);
                        rawContent.AppendFormat("deployment[application_id]={0}&", appID);
                        rawContent.AppendFormat("deployment[description]={0}&", notes);
                        rawContent.AppendFormat("deployment[revision]={0}&", revision);
                        rawContent.AppendFormat("deployment[changelog]={0}&", changes);
                        rawContent.AppendFormat("deployment[user]={0}", user);

                        post.Content = new StringContent(rawContent.ToString());
                        post.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                        
                        var requestTask = client.SendAsync(post);
                        
                        HttpResponseMessage response = requestTask.Result;

                        if(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                            success = true;
                        else
                            message = response.ReasonPhrase;
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