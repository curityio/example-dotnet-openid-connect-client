using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using System;
using exampledotnetopenidconnectclient.App_Start;

namespace exampledotnetopenidconnectclient
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
