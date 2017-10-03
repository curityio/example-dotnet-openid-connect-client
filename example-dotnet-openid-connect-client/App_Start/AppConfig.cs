using System;
using System.Configuration;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace exampledotnetopenidconnectclient.App_Start
{
    public class AppConfig
    {

        private static AppConfig instance;

        private static String client_id;
        private static String client_secret;
        private static String redirect_uri;
        private static String scope;
        private static String authorization_endpoint;
        private static String token_endpoint;
        private static String logout_endpoint;
        private static String revocation_endpoint;
        private static String jwks_uri;
        private static String issuer;
        private static String api_endpoint;
        private static String base_url;

        private AppConfig()
        {
            client_id = ConfigurationManager.AppSettings["client_id"];
            client_secret = ConfigurationManager.AppSettings["client_secret"];
            redirect_uri = ConfigurationManager.AppSettings["redirect_uri"];
            scope = ConfigurationManager.AppSettings["scope"];
            authorization_endpoint = ConfigurationManager.AppSettings["authorization_endpoint"];
            token_endpoint = ConfigurationManager.AppSettings["token_endpoint"];
            logout_endpoint = ConfigurationManager.AppSettings["logout_endpoint"];
            revocation_endpoint = ConfigurationManager.AppSettings["revocation_endpoint"];
            jwks_uri = ConfigurationManager.AppSettings["jwks_uri"];
            issuer = ConfigurationManager.AppSettings["issuer"];
            api_endpoint = ConfigurationManager.AppSettings["api_endpoint"];
            base_url = ConfigurationManager.AppSettings["base_url"];

            if (!String.IsNullOrEmpty(issuer))
            {
                var discoveryClient = new HttpClient();


                var response = discoveryClient.GetAsync(issuer + "/.well-known/openid-configuration").Result;
                if (response.IsSuccessStatusCode)
                {
                    string responseString = response.Content.ReadAsStringAsync().Result;
                    JObject responseJson = JObject.Parse(responseString);

                    authorization_endpoint = responseJson["authorization_endpoint"].ToString();
                    token_endpoint = responseJson["token_endpoint"].ToString();
                    revocation_endpoint = responseJson["revocation_endpoint"].ToString();
                    jwks_uri = responseJson["jwks_uri"].ToString();

                }
            }
        }

        public String GetLogoutEndpoint()
        {
            return logout_endpoint;
        }

        public String GetClientId()
        {
            return client_id;
        }

        public String GetClientSecret()
        {
            return client_secret;
        }

        public String GetRedirectUri()
        {
            return redirect_uri;
        }

        public String GetScope()
        {
            return scope;
        }

        public String GetAuthorizationEndpoint()
        {
            return authorization_endpoint;
        }

        public String GetTokenEndpoint()
        {
            return token_endpoint;
        }

        public String GetRevocationEndpoint()
        {
            return revocation_endpoint;
        }

        public String GetJwksUri()
        {
            return jwks_uri;
        }

        public String GetIssuer()
        {
            return issuer;
        }

        public String GetApiEndpoint()
        {
            return api_endpoint;
        }

        public String GetBaseUrl()
        {
            return base_url;
        }


        public static AppConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AppConfig();
                }
                return instance;
            }
        }
    }
}
