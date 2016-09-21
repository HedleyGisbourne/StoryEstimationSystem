using HG.SoftwareEstimationService.Web.Windsor;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace HG.SoftwareEstimationService.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultPostApi",
                routeTemplate: "api/{controller}/{action}"
            );

            GlobalConfiguration.Configuration.Services.Replace(
            typeof(IHttpControllerActivator),
            new WindsorCompositionRoot());
        }
    }
}