using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace BancoMaster.Backoffice.Api.Extensions
{
    public class ApiExplorerGroupPerVersionConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller == null)
                throw new ArgumentNullException(nameof(controller));

            var controllerNamespace = controller.ControllerType.Namespace;
            controller.ApiExplorer.GroupName = controllerNamespace.Split('.').Last();
        }
    }
}
