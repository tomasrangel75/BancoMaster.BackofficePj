using BancoMaster.Backoffice.Domain.Aws;
using BancoMaster.Backoffice.Domain.Interfaces.Services;
using BancoMaster.Backoffice.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BancoMaster.Backoffice.Service.Extensions
{
    public static class ServiceColletionExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            AddDataServices(services);
            AddBackofficeServices(services);
            AddConfigServices(services);
        }

        private static void AddDataServices(this IServiceCollection services)
        {
            // Repository
            //services.AddSingleton<IGestorTarifasApi, GestorTarifasApiRest>();

            // HTTP
            services.AddHttpClient("api-gestor-tarifas", client =>
            {
                client.BaseAddress = new Uri(GlobalSecrets.WiseConsActions.BaseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = TimeSpan.FromSeconds(20);
            });
        }

        private static void AddBackofficeServices(this IServiceCollection services)
        {
            services.AddSingleton<IUsuarioMasterService, UsuarioMasterService>();

            // Auto Mapper
            // services.AddAutoMapper(typeof(TokenAutenticacaoProfile).Assembly);
        }

        private static void AddConfigServices(this IServiceCollection services)
        {
            services.AddSingleton(GlobalSecrets.WiseConsActions);
        }
    }
}
