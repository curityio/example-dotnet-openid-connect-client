using System.Web.Mvc;

namespace exampledotnetopenidconnectclient.Controllers
{
    public class LogoutController : Controller
    {
        private static string logout_endpoint = App_Start.AppConfig.Instance.GetLogoutEndpoint();

        public ActionResult Index()
        {
            Session.Abandon();

            return Redirect(logout_endpoint);
        }
    }
}
