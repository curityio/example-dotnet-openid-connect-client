using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Configuration;
using Newtonsoft.Json.Linq;
using static System.Convert;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.IO;
using System.Text;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Threading;
using exampledotnetopenidconnectclient.Helpers;
using System.Security.Cryptography;
using System.Net.Http.Headers;

namespace exampledotnetopenidconnectclient.Controllers
{
    public class CallbackController : Controller
    {
        private string client_id = ConfigurationManager.AppSettings["client_id"];
        private string client_secret = ConfigurationManager.AppSettings["client_secret"];
        private string code = ConfigurationManager.AppSettings["client_id"];
        private string redirect_uri = ConfigurationManager.AppSettings["redirect_uri"];
        private string token_endpoint = ConfigurationManager.AppSettings["token_endpoint"];
        private string issuer = ConfigurationManager.AppSettings["issuer"];
        private string jwks_uri = ConfigurationManager.AppSettings["jwks_uri"];

        private JObject id_token_obj;

        static readonly HttpClient client = new HttpClient();

        public ActionResult Index()
        {
            
            string code = Request.QueryString["code"];

            var values = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", client_id},
                { "client_secret", client_secret },
                { "code" , code },
                { "redirect_uri", redirect_uri}
            };

            var content = new FormUrlEncodedContent(values);


            var response = client.PostAsync(token_endpoint, content).Result;
            if (response.IsSuccessStatusCode)
            {
                // by calling .Result you are performing a synchronous call
                var responseContent = response.Content;

                string responseString = responseContent.ReadAsStringAsync().Result;

                saveDataToSession(responseString);

            }

            return Redirect("/");
        }

        public void saveDataToSession(String curityResponse)
        {
            JObject jsonObj = JObject.Parse(curityResponse);

            Session["access_token"] = jsonObj.GetValue("access_token");
            Session["refresh_token"] = jsonObj.GetValue("refresh_token");
            Session["scope"] = jsonObj.GetValue("scope");

            if (jsonObj.GetValue("id_token") != null && isJwtValid(jsonObj.GetValue("id_token").ToString()))
            {
                Session["id_token"] = jsonObj.GetValue("id_token");
                Session["id_token_json0"] = id_token_obj.GetValue("decoded_header").ToString();
                Session["id_token_json1"] = id_token_obj.GetValue("decoded_payload").ToString();
            }

            Session["token_type"] = jsonObj.GetValue("token_type");

            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            Session["access_token_expires"] = Int32.Parse(jsonObj.GetValue("expires_in").ToString()) + secondsSinceEpoch;

        }

       
        public String safeDecodeBase64(String str)
        {
            return System.Text.Encoding.UTF8.GetString(
                getPaddedBase64String(str));
        }

		private byte[] getPaddedBase64String(string base64Url)
		{
			string padded = base64Url.Length % 4 == 0 ? base64Url : base64Url + "====".Substring(base64Url.Length % 4);
			string base64 = padded.Replace("_", "/")
								  .Replace("-", "+");
			return FromBase64String(base64);
		}

        public bool isJwtValid(String jwt)
        {
            string[] jwtParts = jwt.Split('.');

            String decodedHeader = safeDecodeBase64(jwtParts[0]);
            id_token_obj = new JObject
            {
                {"decoded_header", decodedHeader },
                {"decoded_payload", safeDecodeBase64(jwtParts[1])}
            };

            String keyId = JObject.Parse(decodedHeader).GetValue("kid").ToString();

            var jwksclient = new HttpClient();

            jwksclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = jwksclient.GetAsync(jwks_uri).Result;
			if (response.IsSuccessStatusCode)
			{
				// by calling .Result you are performing a synchronous call
				var responseContent = response.Content;

				string responseString = responseContent.ReadAsStringAsync().Result;

                JToken keyFound = null;
                foreach (JToken key in JObject.Parse(responseString).GetValue("keys").ToArray()) {
                    if (key["kid"].ToString().Equals(keyId)){
                        keyFound = key;
                    }
                }
                if (keyFound != null)
                {
                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                    rsa.ImportParameters(
                      new RSAParameters()
                      {
                        Modulus = getPaddedBase64String(keyFound["n"].ToString()),
                        Exponent = getPaddedBase64String(keyFound["e"].ToString())
                      });

                    SHA256 sha256 = SHA256.Create();
                    byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(jwtParts[0] + '.' + jwtParts[1]));

                    RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
                    rsaDeformatter.SetHashAlgorithm("SHA256");
                    if (rsaDeformatter.VerifySignature(hash, getPaddedBase64String(jwtParts[2])))
                        return true;
                }
			}
            return false;
		}
    }
}