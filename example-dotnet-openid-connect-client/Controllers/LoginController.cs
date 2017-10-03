using System;
using System.Web.Mvc;
using System.Configuration;

namespace exampledotnetopenidconnectclient.Controllers
{
    public class LoginController : Controller
    {
        private string client_id = ConfigurationManager.AppSettings["client_id"];
        private string authorization_endpoint = ConfigurationManager.AppSettings["authorization_endpoint"];
        private string scope = ConfigurationManager.AppSettings["scope"];
        private string redirect_uri = ConfigurationManager.AppSettings["redirect_uri"];

        public ActionResult Index()
        {
            return Redirect(authorization_endpoint + "?client_id="
               + client_id + "&response_type=code"
               + "&scope=" + scope + "&redirect_uri=" + redirect_uri);
        }
    }
}
