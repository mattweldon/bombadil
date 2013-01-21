using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Bombadil.Sandbox
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Page",
                url: "page/{title}",
                defaults: new { controller = "Home", action = "Page", title = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Post",
                url: "post/{title}",
                defaults: new { controller = "Home", action = "Post", title = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}