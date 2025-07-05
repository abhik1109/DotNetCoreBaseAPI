using DotNetCoreBaseAPI.Extensions;
using DotNetCoreBaseAPI.Models;
using DotNetCoreBaseAPI.Utilities.Enums;
using DotNetCoreBaseAPI.Utilities.Policies;
using HealthChecks.UI.Client;
using HealthChecks.UI.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DotNetCoreBaseAPI
{
	public static class ProgramBase
	{

		public static WebApplicationBuilder CreateWebApplicationBuilder(this WebApplicationBuilder builder, string[] args,params object[] additionalInfo)
		{
			var env = builder.Environment;
			var isDev = !env.IsProduction();

			//Read local json settings
			//var currentDirectory = Environment.CurrentDirectory;
			//using (StreamReader sr=new StreamReader($"{currentDirectory}\\appSettingsBase.json"))
			//{
			//	string json = sr.ReadToEnd();
			//	var baseSettings = JsonConvert.DeserializeObject(json);
			//}

				//Add configuration files			
				var config = new ConfigurationBuilder()
					.SetBasePath(AppContext.BaseDirectory)
					//.AddJsonFile($"appSettingsBase.json", optional: false, reloadOnChange: true)
					.AddJsonFile($"appSettings.json", optional: false, reloadOnChange: true)
					.AddJsonFile($"appSettings.{env.EnvironmentName}.json", optional: true)
					.AddEnvironmentVariables()
					.Build();
			builder.Configuration.AddConfiguration(config);

			//Configure Serilog logger
			if (additionalInfo.FirstOrDefault(x => x.GetType() == typeof(LoggerProvider)) != default)
			{
				var loggerProvider = (LoggerProvider)(additionalInfo.FirstOrDefault(x => x.GetType() == typeof(LoggerProvider))??LoggerProvider.NONE);
				builder.RegisterLogging(loggerProvider);
			}			

			//Register auth services
			builder.Services.RegisterAuthenticationServices(builder.Configuration);

			//Configure swagger
			builder.Services.AddSwaggerGen();

			//Register Versioning
			builder.Services.RegisterApiVersioning();

			//Add named http client factory
			if (additionalInfo.FirstOrDefault(x => x.GetType() == typeof(HttpClientMetadata)) != default)
			{
				HttpClientMetadata httpClientMetadata = (HttpClientMetadata)additionalInfo.FirstOrDefault(x => x.GetType() == typeof(HttpClientMetadata));
				foreach(var client in httpClientMetadata?.httpClients)
				{
					builder.Services.AddHttpClient(client.ClientName, c =>
					{
						c.BaseAddress = new Uri(client.BaseUri);
						foreach(var header in client.RequestHeaders)
						{
							c.DefaultRequestHeaders.Add(header.Key, header.Value);
						}
					});
				}
			}

			//Register client retry policies
			builder.Services.AddSingleton<ClientRetryPolicy>(new ClientRetryPolicy());

			builder.Services.AddControllers(options =>
			{
				// Enable 406 Not Acceptable status code
				options.ReturnHttpNotAcceptable = true;
			}).AddXmlSerializerFormatters();
			builder.Services.AddEndpointsApiExplorer();
			
			builder.Services.AddHttpContextAccessor();

			//Register health checks
			builder.Services.RegisterHealthChecks(builder.Configuration);

			return builder;
		}

		public static WebApplication BuildBaseHttpPipeline(this WebApplication app, IConfiguration configuration)
		{
			var isDev = !app.Environment.IsProduction();

			if (isDev)
			{
				app.UseSwagger();
				app.UseSwaggerUI();
				app.UseDeveloperExceptionPage();
			}
			
			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();
			app.MapHealthChecks("api/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
			{
				Predicate = _ => true,
				ResponseWriter=UIResponseWriter.WriteHealthCheckUIResponse
			});
			app.UseHealthChecksUI(delegate (Options options)
			{
				options.UIPath = "/heathcheck-ui";
				options.AddCustomStylesheet("./HealthCheck/Custom.css");
			});
			return app;
		}
				
	}
}
