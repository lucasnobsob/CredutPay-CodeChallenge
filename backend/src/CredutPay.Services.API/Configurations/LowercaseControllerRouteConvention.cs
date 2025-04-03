using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace CredutPay.Services.API.Configurations
{
    public class LowercaseControllerRouteConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            // Convert the controller name to lowercase
            var controllerName = controller.ControllerName;
            if (!string.IsNullOrEmpty(controllerName))
            {
                controllerName = char.ToLower(controllerName[0]) + controllerName.Substring(1);
            }

            // Update the route template for all actions in the controller
            foreach (var selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel != null)
                {
                    selector.AttributeRouteModel.Template = selector.AttributeRouteModel.Template
                        .Replace("[controller]", controllerName);
                }
            }
        }
    }
}
