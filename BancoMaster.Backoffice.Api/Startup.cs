using BancoMaster.Backoffice.Api.Extensions;
using BancoMaster.Backoffice.Domain.Aws;
using BancoMaster.Backoffice.Domain.Exceptions;
using BancoMaster.Backoffice.Service.Extensions;
using BancoMaster.LogManager.Extensions;
using BancoMaster.LogManager.Middleware;
using BancoMaster.LogManager.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog.Events;
using System.Reflection;
using System.Text.Json;

namespace BancoMaster.Backoffice.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServices();

            services.AddControllers(c => c.Conventions.Add(new ApiExplorerGroupPerVersionConvention()))
                .AddDataAnnotationsLocalization()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });

            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(x => x.Name);

                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "API Proxy Backoffice PJ - Banco Master",
                        Version = "v1",
                        Description = "API Proxy Backoffice PJ. Proxy de APIs de cadastro de usuário master.",
                        Contact = new OpenApiContact
                        {
                            Name = "Manual de Desenvolvimento",
                            Url = new Uri("https://bmaxima.sharepoint.com/:w:/s/TI/ET3e49--O6lItd96cBluB7ABn5bxezFpEDvRnu0HPeQtOA?e=8dcFju")
                        },
                        TermsOfService = new Uri("https://www.bancomaster.com.br/outras-informacoes/termos-de-uso"),
                        License = new OpenApiLicense
                        {
                            Name = "Banco Master",
                            Url = new Uri("https://www.bancomaster.com.br"),
                        }
                    });

                #region sec
                //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    Scheme = "Bearer",
                //    In = ParameterLocation.Header,
                //    Name = "Authorization",
                //    Type = SecuritySchemeType.ApiKey,
                //    Description = "Token de autenticação Bearer. Exemplo: \"Bearer {token}\"",
                //});

                //c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                //            Scheme = "oauth2",
                //            Name = "Bearer",
                //            In = ParameterLocation.Header
                //        },
                //        new List<string>()
                //    }
                //});

                #endregion

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddHealthChecks();

            // Auth
            //services.AddHttpContextAccessor();
            //AddCognitoAuthentication(services);

            ConfigurarLog(services);
        }

        public static void Configure(IApplicationBuilder app, ILogger<Startup> logger)
        {
            // Middleware de correlationID para gerar UUID integrado entre Sentry, Response API e Log Kibana
            app.UseMiddleware<CorrelationIdMiddleware>();

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
                c.SerializeAsV2 = true;
            });

            app.UseSwaggerUI();

            app.UseHsts();

            app.UseExceptionHandler(options =>
            {
                options.Run(async context =>
                {
                    string message;
                    var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                    if (ex == null)
                    {
                        return;
                    }
                    else if (ex is MasterException legacyEx)
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;

                        var error = new { legacyEx.CodigoErro, Mensagem = ex.Message };
                        message = JsonConvert.SerializeObject(error, Formatting.Indented);
                        logger?.LogWarning(message);
                    }
                    else
                    {
                        logger?.LogError(ex, "Ocorreu um erro inesperado na chamada da API.");
                        var error = new { context.Response.StatusCode, Mensagem = ex.Message };
                        message = JsonConvert.SerializeObject(error, Formatting.Indented);
                    }

                    context.Response.ContentType = "application/json; charset=utf-8";
                    await context.Response.WriteAsync(message);
                });
            });

            app.UseHealthChecks("/health")
                .UseSwagger()
                .UseStaticFiles()
                .UseHttpsRedirection();

            //app.UseAuthentication();
            
            app.UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints => endpoints.MapControllers());
        }

        private void ConfigurarLog(IServiceCollection services)
        {
            var env = Configuration.GetValue<string>("ENVIRONMENT");

            var kibanaOptions = new ConfigureLogsOptions()
            {
                Application = "proxy-cadastro-usuario-api",
                SentryDsn = "",
                ElasticUri = GlobalSecrets.ElasticSearchUrl,
                ElasticSearchIndex = "proxy-cadastro-usuario-api",
                LogEventLevel = LogEventLevel.Information
            };

            services.ConfigureLogManager(kibanaOptions);
        }
        
    }
}