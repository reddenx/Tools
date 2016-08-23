using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using SMT.Utilities.DynamicApi.Api;

namespace App.WebTesting
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
		{
			var type = DynamicApiAssembler.SetupTypes(
				() => HttpContext.Current.Request.RequestContext.RouteData.Values,
				() => new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd(),
				typeof(System.Web.Http.HttpPostAttribute),
				typeof(System.Web.Http.ApiController),
				new[] { typeof(System.Web.Http.Controllers.IHttpController), typeof(IDisposable) },
				Assembly.GetExecutingAssembly());

			var type2 = typeof(Controllers.RouringBaseController);

			var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(t => t.BaseType != null && t.BaseType.Name == "ApiController").ToArray();
			var types2 = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType != null && t.BaseType.Name == "ApiController").ToArray();

			AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}