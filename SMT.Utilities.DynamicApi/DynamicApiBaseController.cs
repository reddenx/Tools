﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;

namespace SMT.Utilities.DynamicApi
{
    //server side: given the interface and implementation of the object that is proxied, build out a controller that calls into this object
    public class DynamicApiBaseController : ApiController
    {
        private static Type[] DynamicApis;

        //called during api registration
        public static void RegisterDynamicRoutes(HttpRouteCollection routes, string prefix)
        {
            //get all interfaces with the attribute
            var dynamicApis = Assembly.GetCallingAssembly().GetTypes()
                .Where(type => type.GetInterfaces().Any(i => i.GetCustomAttribute(typeof(DynamicApiAttribute)) != null));

            //gte all implementors of that attribute and register them
            foreach (var api in dynamicApis)
            {
                var routeAttr = api.GetInterfaces().First(i => i.GetCustomAttribute(typeof(DynamicApiAttribute)) != null).GetCustomAttribute(typeof(DynamicApiAttribute)) as DynamicApiAttribute;
                var route = routeAttr.RouteName;
                var template = prefix + "/{DynamicObject}/{DestinationMethod}";
                routes.MapHttpRoute(route, template,
                    new { controller = "DynamicApiBase", action = "RunDynamic" });
            }

            DynamicApis = dynamicApis.ToArray();
        }

        //allow all the things!
        [HttpGet]
        [HttpPost]
        [HttpPut]
        [HttpDelete]
        [HttpPatch]
        public object RunDynamic()
        {
            //build up routing information since this is the generic input for everything
            var routeData = Request.GetRouteData();
            var destObjName = routeData.Values["DynamicObject"] as string;
            var destMethod = routeData.Values["DestinationMethod"] as string;


            if (destObjName == null || destMethod == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var destObjType = DynamicApis.SingleOrDefault(type => (type.GetInterfaces().First(i => i.GetCustomAttribute(typeof(DynamicApiAttribute)) != null).GetCustomAttribute(typeof(DynamicApiAttribute)) as DynamicApiAttribute).RouteName == destObjName);

            if (destObjType == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var destObj = Activator.CreateInstance(destObjType);

            var objMethod = destObjType.GetMethod(destMethod);

            var methodParameters = objMethod.GetParameters();
            var parameters = new List<object>();

            //deserialize input parameters
            if (methodParameters.Any())
            {
                var requestBodyString = Request.Content.ReadAsStringAsync().Result;


                var inputList = JArray.Parse(requestBodyString);

                for (int i = 0; i < methodParameters.Length; ++i)
                {
                    parameters.Add(inputList[i].ToObject(methodParameters[i].ParameterType));
                }
            }


            var response = objMethod.Invoke(destObj, parameters.ToArray());
            return response;
        }

        public override System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> ExecuteAsync(System.Web.Http.Controllers.HttpControllerContext controllerContext, System.Threading.CancellationToken cancellationToken)
        {
            return base.ExecuteAsync(controllerContext, cancellationToken);
        }
    }
}
