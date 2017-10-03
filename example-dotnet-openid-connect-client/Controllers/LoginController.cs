using System.Web.Mvc;

namespace exampledotnetopenidconnectclient.Controllers
{
    public class LoginController : Controller
    {
		private static string start_oauth_endpoint = Helpers.Client.Instance.GetAuthnReqUrl();

		public ActionResult Index()
        {
            return Redirect(start_oauth_endpoint);
        }
    }
}
