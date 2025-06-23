using DotNetCoreBaseAPI.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Reflection;

namespace DotNetCoreBaseAPI
{
	public static class ProgramBase
	{

		public static WebApplicationBuilder CreateWebApplicationBuilder(this WebApplicationBuilder builder, string[] args)
		{
			var env = builder.Environment;
			var isDev = !env.IsProduction();

			//Add configuration files
			var workingDirectory=Directory.GetCurrentDirectory();
			var config = new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
				.AddJsonFile($"appSettingsBase.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appSettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appSettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables()
				.Build();
			builder.Configuration.AddConfiguration(config);

			//Configure Serilog logger
			var logger = new LoggerConfiguration()
				.ReadFrom.Configuration(builder.Configuration)
				.Enrich.FromLogContext()
				.CreateLogger();

			builder.Logging.ClearProviders();
			builder.Logging.AddSerilog(logger);

			//Register auth services
			builder.Services.RegisterAuthenticationServices(builder.Configuration);

			//Configure swagger
			builder.Services.AddSwaggerGen();

			//Register Versioning
			builder.Services.RegisterApiVersioning();


			builder.Services.AddControllers();
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
