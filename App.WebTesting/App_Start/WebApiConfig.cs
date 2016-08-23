using SMT.Utilities.DynamicApi.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web;
using System.IO;

namespace App.WebTesting
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var routes = DynamicApiAssembler.GetRoutes("derp");
            foreach (var route in routes)
            {
                config.Routes.MapHttpRoute(
                    name: route.Name,
                    routeTemplate: route.Template,
                    defaults: route.Defaults);
            }

			config.Routes.MapHttpRoute(
				name: "derprefgretg",
				routeTemplate: "api/{controller}/{action}");
        }
    }
}
