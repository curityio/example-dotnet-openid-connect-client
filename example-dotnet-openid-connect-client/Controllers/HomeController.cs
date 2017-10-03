using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Configuration;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace exampledotnetopenidconnectclient.Controllers
{
    public class HomeController : Controller
    {
        private string client_id = System.Configuration.ConfigurationManager.AppSettings["client_id"];
        private string client_secret = System.Configuration.ConfigurationManager.AppSettings["client_secret"];
        private string token_endpoint = System.Configuration.ConfigurationManager.AppSettings["token_endpoint"];
        private string revocation_endpoint = System.Configuration.ConfigurationManager.AppSettings["revocation_endpoint"];
        private string api_endpoint = System.Configuration.ConfigurationManager.AppSettings["api_endpoint"];

        static readonly HttpClient client = new HttpClient();

        public ActionResult Index()
        {

            ViewData["server_name"] = token_endpoint;

            return View();

        }

        public ActionResult Refresh()
        {

            String refresh_token = Session["refresh_token"].ToString();
            var values = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refresh_token },
                { "client_secret", client_secret },
                { "client_id" , client_id }
            };

            var content = new FormUrlEncodedContent(values);

            var refreshClient = new HttpClient();
            var response = refreshClient.PostAsync(token_endpoint, content).Result;
            if (response.IsSuccessStatusCode)
            {
                // by calling .Result you are performing a synchronous call
                var responseContent = response.Content;

                string responseString = responseContent.ReadAsStringAsync().Result;
                JObject jsonObj = JObject.Parse(responseString);
                Session["access_token"] = jsonObj.GetValue("access_token");
                Session["refresh_token"] = jsonObj.GetValue("refresh_token");

                return Redirect("/");
            }
            Session["error"] = "Could not refresh Access Token";
            return Redirect("/");

        }

        public ActionResult Revoke()
        {
            String refresh_token = Session["refresh_token"].ToString();
            var values = new Dictionary<string, string>
            {
                { "token", refresh_token },
                { "client_secret", client_secret },
                { "client_id" , client_id }
            };

            var content = new FormUrlEncodedContent(values);
            var revokeClient = new HttpClient();

            var response = revokeClient.PostAsync(revocation_endpoint, content).Result;
            if (response.IsSuccessStatusCode)
            {
                // by calling .Result you are performing a synchronous call
                var responseContent = response.Content;

                string responseString = responseContent.ReadAsStringAsync().Result;
                Session["refresh_token"] = null;

                return Redirect("/");
            }
            Session["error"] = "Could not revoke Access Token";
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
