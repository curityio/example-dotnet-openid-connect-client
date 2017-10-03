using System;
using System.Collections.Generic;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace exampledotnetopenidconnectclient.Helpers
{
    public class Client
    {
        private static Client instance;
        private static App_Start.AppConfig _config = App_Start.AppConfig.Instance;

        private string client_id;
        private string client_secret;
        private string redirect_uri;
        private string token_endpoint;
        private string issuer;
        private string jwks_uri;
        private string revocation_endpoint;
        private string authorization_endpoint;
        private string scope;

        private Client()
        {
            client_id = _config.GetClientId();
            client_secret = _config.GetClientSecret();
            redirect_uri = _config.GetRedirectUri();
            token_endpoint = _config.GetTokenEndpoint();
            issuer = _config.GetIssuer();
            jwks_uri = _config.GetJwksUri();
            revocation_endpoint = _config.GetRevocationEndpoint();
            authorization_endpoint = _config.GetAuthorizationEndpoint();
            scope = _config.GetScope();
        }

        public bool Revoke(String refresh_token)
        {
            var values = new Dictionary<string, string>
            {
                { "token", refresh_token },
                { "client_secret", client_secret },
                { "client_id" , client_id }
            };

            HttpClient revokeClient = new HttpClient();
            var content = new FormUrlEncodedContent(values);
            var response = revokeClient.PostAsync(revocation_endpoint, content).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
                string responseString = responseContent.ReadAsStringAsync().Result;

                return true;
            }

            return false;
        }

        public String Refresh(String refresh_token)
        {
            var values = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refresh_token },
                { "client_secret", client_secret },
                { "client_id" , client_id }
            };

            HttpClient refreshClient = new HttpClient();
            var content = new FormUrlEncodedContent(values);
            var response = refreshClient.PostAsync(token_endpoint, content).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;

                return responseContent.ReadAsStringAsync().Result;
            }

            return null;
        }

        public String GetAuthnReqUrl()
        {
            return authorization_endpoint + "?client_id="
               + client_id + "&response_type=code"
               + "&scope=" + scope + "&redirect_uri=" + redirect_uri;
        }

        public String GetToken(String code)
        {
            var values = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", client_id},
                { "client_secret", client_secret },
                { "code" , code },
                { "redirect_uri", redirect_uri}
            };


            HttpClient tokenClient = new HttpClient();
            var content = new FormUrlEncodedContent(values);
            var response = tokenClient.PostAsync(token_endpoint, content).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;

                return responseContent.ReadAsStringAsync().Result; ;
            }

            return null;
        }

        public static Client Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Client();
                }
                return instance;
            }
        }
    }
}
