using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Exercise3
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("displayPicture", "display/{ip}/{port}",
            defaults: new { controller = "First", action = "displayPicture" });

            routes.MapRoute("connectServer", "displayConn/{ip}/{port}",
            defaults: new { controller = "First", action = "connectServer" });

            routes.MapRoute("display", "display/{ip}/{port}/{time}",
            defaults: new { controller = "First", action = "displayWay" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "First", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
