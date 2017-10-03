using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace exampledotnetopenidconnectclient.Controllers
{
    public class LogoutController : Controller
    {
        private string logout_endpoint = ConfigurationManager.AppSettings["logout_endpoint"];

        public ActionResult Index()
        {
            Session.Abandon();

            return Redirect(logout_endpoint);
        }
    }
}
