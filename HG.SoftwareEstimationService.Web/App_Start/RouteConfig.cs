using System.Web.Mvc;
using System.Web.Routing;

namespace HG.SoftwareEstimationService.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Layers",
                url: "SystemConfig/{systemId}/Layers",
                defaults: new { controller = "SystemConfig", action = "Layers", systemId = 0 }
            );

            routes.MapRoute(
                name: "Subsystems",
                url: "SystemConfig/{systemId}/Subsystems",
                defaults: new { controller = "SystemConfig", action = "Subsystems", systemId = 0 }
            );

            routes.MapRoute(
                name: "Enumerations",
                url: "SystemConfig/{systemId}/Enumerations",
                defaults: new { controller = "SystemConfig", action = "Enumerations", systemId = 0 }
            );

            routes.MapRoute(
                name: "ComplexityComponents",
                url: "SystemConfig/{systemId}/ComplexityComponents",
                defaults: new { controller = "SystemConfig", action = "ComplexityComponents", systemId = 0 }
            );

            routes.MapRoute(
                name: "SystemConfig",
                url: "SystemConfig/{systemId}",
                defaults: new { controller = "SystemConfig", action = "Index", systemId = 0 }
            );

            routes.MapRoute(
                name: "Systems",
                url: "Administration/Systems",
                defaults: new { controller = "Administration", action = "Systems" }
            );

            routes.MapRoute(
                name: "Languages",
                url: "Administration/Languages",
                defaults: new { controller = "Administration", action = "Languages" }
            );

            routes.MapRoute(
                name: "ArchitecturalConcerns",
                url: "Administration/ArchitecturalConcerns",
                defaults: new { controller = "Administration", action = "ArchitecturalConcerns" }
            );

            routes.MapRoute(
                name: "Stories",
                url: "{controller}/{id}",
                defaults: new { controller = "Stories", action = "Index", Id = @"\d+" }
            );

            routes.MapRoute(
                name: "AllStories",
                url: "{controller}/{id}/{allStories}",
                defaults: new { controller = "Stories", action = "Index", Id = @"\d+" }
            );

            routes.MapRoute(
                name: "Ticket",
                url: "{controller}/{systemId}/Ticket/{ticketId}",
                defaults: new { controller = "Stories", action = "Observations" }
            );
        }
    }
}