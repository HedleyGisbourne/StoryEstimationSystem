using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace HG.SoftwareEstimationService.Web
{
    public class ControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            try
            {
                if (controllerType == null)
                    throw new ArgumentNullException("controllerType");

                if (!typeof(IController).IsAssignableFrom(controllerType))
                    throw new ArgumentException(string.Format(
                        "Type requested is not a controller: {0}",
                        controllerType.Name),
                        "controllerType");

                return CompositionRoot.CompositionRoot.Resolve(controllerType) as IController;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}