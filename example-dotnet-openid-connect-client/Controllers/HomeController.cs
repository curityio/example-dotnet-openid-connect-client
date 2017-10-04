using System;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace exampledotnetopenidconnectclient.Controllers
{
    
    public class HomeController : Controller
    {
        private static Helpers.Client _client = Helpers.Client.Instance;
        private string api_endpoint = App_Start.AppConfig.Instance.GetApiEndpoint();
        private string iss = App_Start.AppConfig.Instance.GetIssuer();

        static readonly HttpClient client = new HttpClient();

        public ActionResult Index()
        {

            ViewData["server_name"] = iss;
            if (Session["error"] != null)
            {
                ViewData["error"] = Session["error"];
                Session["error"] = null;
            }

            return View();

        }

        public ActionResult Refresh()
        {
            String responseString = _client.Refresh(Session["refresh_token"].ToString());
            if (String.IsNullOrEmpty(responseString))
            {
                Session["error"] = "Could not refresh Access Token";
            }
            else
            {
                JObject jsonObj = JObject.Parse(responseString);
                Session["access_token"] = jsonObj.GetValue("access_token");
                Session["refresh_token"] = jsonObj.GetValue("refresh_token");
            }

            return Redirect("/");
        }

        public ActionResult Revoke()
        {
            if (_client.Revoke(Session["refresh_token"].ToString()))
            {
                Session["refresh_token"] = null;
            }
            else
            {
                Session["error"] = "Could not revoke Access Token";
            }

            return Redirect("/");
        }

        public ActionResult CallApi()
        {

            String access_token = Session["access_token"].ToString();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);

            var response = client.GetAsync(api_endpoint).Result;
            Session["api_response_status_code"] = response.StatusCode;

            var responseContent = response.Content;

            string responseString = responseContent.ReadAsStringAsync().Result;
            Session["api_response_data"] = responseString;

            return Redirect("/");
        }
    }
}
