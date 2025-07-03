using DotNetCoreBaseAPI.Extensions;
using DotNetCoreBaseAPI.Models;
using DotNetCoreBaseAPI.Utilities.Enums;
using DotNetCoreBaseAPI.Utilities.Policies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNetCoreBaseAPI
{
	public static class ProgramBase
	{

		public static WebApplicationBuilder CreateWebApplicationBuilder(this WebApplicationBuilder builder, string[] args,params object[] additionalInfo)
		{
			var env = builder.Environment;
			var isDev = !env.IsProduction();

			//Add configuration files
			//var workingDirectory=AppContext.
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
			//app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();

			return app;
		}
				
	}
}
