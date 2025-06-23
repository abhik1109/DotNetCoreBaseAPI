using DotNetCoreBaseAPI.Extensions;
using DotNetCoreBaseAPI.Handlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace DotNetCoreBaseAPI
{
	public static class ProgramBase
	{

		public static WebApplicationBuilder CreateWebApplicationBuilder(this WebApplicationBuilder builder, string[] args)
		{
			var env = builder.Environment;
			var isDev = !env.IsProduction();

			//Add configuration files
			var config = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
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

			//Register DB Services
			builder.Services.RegisterDatabaseService(builder.Configuration);

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
