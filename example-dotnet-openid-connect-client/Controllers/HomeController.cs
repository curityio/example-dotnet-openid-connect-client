/*
 * Copyright (C) 2016 Curity AB.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;
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
