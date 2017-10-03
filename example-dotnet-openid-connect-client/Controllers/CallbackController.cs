using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using exampledotnetopenidconnectclient.Helpers;
using Newtonsoft.Json.Linq;
using static System.Convert;

namespace exampledotnetopenidconnectclient.Controllers
{
    public class CallbackController : Controller
    {
        private static Helpers.Client _client = Helpers.Client.Instance;
        private string issuer = App_Start.AppConfig.Instance.GetIssuer();
        private string jwks_uri = App_Start.AppConfig.Instance.GetJwksUri();

        private JObject id_token_obj;

        static readonly HttpClient client = new HttpClient();

        public ActionResult Index()
        {
            string responseString = _client.GetToken(Request.QueryString["code"]);

            if (!String.IsNullOrEmpty(responseString))
            {
                SaveDataToSession(responseString);

            }

            return Redirect("/");
        }

        public void SaveDataToSession(String curityResponse)
        {
            JObject jsonObj = JObject.Parse(curityResponse);

            Session["access_token"] = jsonObj.GetValue("access_token");
            Session["refresh_token"] = jsonObj.GetValue("refresh_token");
            Session["scope"] = jsonObj.GetValue("scope");

            if (jsonObj.GetValue("id_token") != null && IsJwtValid(jsonObj.GetValue("id_token").ToString()))
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


        public String SafeDecodeBase64(String str)
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

        public bool IsJwtValid(String jwt)
        {
            string[] jwtParts = jwt.Split('.');

            String decodedHeader = SafeDecodeBase64(jwtParts[0]);
            id_token_obj = new JObject
            {
                {"decoded_header", decodedHeader },
                {"decoded_payload", SafeDecodeBase64(jwtParts[1])}
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
                foreach (JToken key in JObject.Parse(responseString).GetValue("keys").ToArray())
                {
                    if (key["kid"].ToString().Equals(keyId))
                    {
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