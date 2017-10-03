using System;
using System.Configuration;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace exampledotnetopenidconnectclient.App_Start
{
    public class AppConfig
    {
        //private static string discovery_url = ConfigurationManager.AppSettings["discovery_url"];
        //private Dictionary<string, string> dict;

        public static void init()
        {
   //         dict = new Dictionary<string, string>
   //         {
			//	{"client_id",  ConfigurationManager.AppSettings["client_id"]},
			//	{"client_secret",  ConfigurationManager.AppSettings["client_secret"]},
			//	{"redirect_uri",  ConfigurationManager.AppSettings["redirect_uri"]},
			//	{"scope",  ConfigurationManager.AppSettings["scope"]},
			//	{"authorization_endpoint",  ConfigurationManager.AppSettings["authorization_endpoint"]},
			//	{"token_endpoint",  ConfigurationManager.AppSettings["token_endpoint"]},
			//	{"logout_endpoint",  ConfigurationManager.AppSettings["logout_endpoint"]},
			//	{"revocation_endpoint",  ConfigurationManager.AppSettings["revocation_endpoint"]},
			//	{"jwks_uri",  ConfigurationManager.AppSettings["jwks_uri"]},
			//	{"issuer",  ConfigurationManager.AppSettings["issuer"]},
			//	{"api_endpoint",  ConfigurationManager.AppSettings["api_endpoint"]},
			//	{"base_url",  ConfigurationManager.AppSettings["base_url"]}
			//};

    //        if (!String.IsNullOrEmpty(discovery_url))
    //        {
    //            var discoveryClient = new HttpClient();


    //            var response = discoveryClient.GetAsync(discovery_url).Result;
    //            if (response.IsSuccessStatusCode)
    //            {
    //                string responseString = response.Content.ReadAsStringAsync().Result;
    //                JObject responseJson = JObject.Parse(responseString);

					

				//	//config.AppSettings.Settings["authorization_endpoint"].Value = responseJson["authorization_endpoint"].ToString();
				//	config.AppSettings.Settings["jwks_uri"].Value = responseJson["jwks_uri"].ToString();

					
				//	//System.Configuration.ConfigurationManager.AppSettings.Add("introspection_endpoint", responseJson["introspection_endpoint"].ToString());
				//	//System.Configuration.ConfigurationManager.AppSettings.Add("authorization_endpoint", responseJson["authorization_endpoint"].ToString());
				//	//System.Configuration.ConfigurationManager.AppSettings["issuer"] = responseJson["issuer"].ToString();
				//	//System.Configuration.ConfigurationManager.AppSettings["authorization_endpoint"] = responseJson["authorization_endpoint"].ToString();
				//	//System.Configuration.ConfigurationManager.AppSettings["token_endpoint"] = responseJson["token_endpoint"].ToString();
				//	//System.Configuration.ConfigurationManager.AppSettings["jwks_uri"] = responseJson["jwks_uri"].ToString();
				//	//System.Configuration.ConfigurationManager.AppSettings["revocation_endpoint"] = responseJson["revocation_endpoint"].ToString();
				//	//System.Configuration.ConfigurationManager.AppSettings["userinfo_endpoint"] = responseJson["userinfo_endpoint"].ToString();
				//}
            //}
        }

        //public static String get(String key)
        //{         
        //    return dict[key]; 
        //}
    }
}
