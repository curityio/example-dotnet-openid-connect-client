/*
 * Copyright (C) 2017 Curity AB.
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
using System.Collections.Generic;
using System.Net.Http;

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

        public void Revoke(String refresh_token)
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

                return;
            }

            throw new OAuthClientException("Could not revoke the refresh token");
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

            throw new OAuthClientException("Could not refresh the tokens");
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

                return responseContent.ReadAsStringAsync().Result;
            }

            throw new OAuthClientException("Token request failed with status code: " + response.StatusCode);
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
