using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using HG.SoftwareEstimationService.Web.Windsor;

namespace HG.SoftwareEstimationService.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ControllerBuilder.Current.SetControllerFactory(typeof(ControllerFactory));

            WinsorInstaller controllerInstaller = new WinsorInstaller();
            CompositionRoot.CompositionRoot.InstallAsembly(controllerInstaller);

            // http://stackoverflow.com/questions/16281366/asp-net-mvc-rendering-seems-slow
            // slow performance
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
        }
    }
}
