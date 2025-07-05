using Asp.Versioning;
using DotNetCoreBaseAPI.Handlers;
using DotNetCoreBaseAPI.Heathchecks;
using DotNetCoreBaseAPI.Utilities.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace DotNetCoreBaseAPI.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection RegisterAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
		{
			var scheme = configuration.GetValue<string>("Authentication:Scheme") ?? "".ToUpper();

			switch (scheme)
			{
				case "TOKEN":
				case "JWT":
					services.AddAuthentication(options =>
					{
						options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
						options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
					})
					.AddJwtBearer(jwtOptions =>
					{
						var issuer = configuration.GetValue<string>("JwtConfig:Issuer");
						var key = configuration.GetValue<string>("JwtConfig:Key");
						var keyBytes = Encoding.ASCII.GetBytes(key);
						jwtOptions.SaveToken = true;
						jwtOptions.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
						{
							IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
							ValidIssuer = issuer,
							ValidAudience = issuer,
							ValidateLifetime = true,
							ValidateAudience = true,
							ValidateIssuer = true,
							ValidateIssuerSigningKey = true
						};
					});
					break;

				case "OAUTH":
					services.AddAuthentication()
						.AddGoogle(gOptions =>
						{
							gOptions.ClientId = configuration.GetValue<string>("Authentication:GoogleId") ?? "";
							gOptions.ClientSecret = configuration.GetValue<string>("Authentication:GoogleSecret") ?? "";
						})
						.AddMicrosoftAccount(msOptions =>
						{
							msOptions.ClientId = configuration.GetValue<string>("Authentication:GoogleId") ?? "";
							msOptions.ClientSecret = configuration.GetValue<string>("Authentication:GoogleSecret") ?? "";
						})
						.AddFacebook(fbOptions =>
						{
							fbOptions.ClientId = configuration.GetValue<string>("Authentication:FacebookId") ?? "";
							fbOptions.ClientSecret = configuration.GetValue<string>("Authentication:FacebookSecret") ?? "";
						});
					break;

				case "BASIC":
					services.AddAuthentication("BasicAuthentication")
						.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandlerBase>("BasicAuthentication", null);
					break;
				default:
					services.AddAuthentication();
					break;
			}

			services.AddAuthorization();
			return services;
		}
		public static IServiceCollection RegisterApiVersioning(this IServiceCollection services)
		{
			services.AddApiVersioning(options =>
			{
				options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.ReportApiVersions = true; //Header info

				options.ApiVersionReader = ApiVersionReader.Combine(
					new QueryStringApiVersionReader("api-version"),
					new HeaderApiVersionReader("x-api-version"),
					new MediaTypeApiVersionReader("ver")
					);
			}).AddApiExplorer(options =>
			{
				options.GroupNameFormat = "'v'VVV";
				options.SubstituteApiVersionInUrl = true;
			});

			return services;
		}

		public static IServiceCollection RegisterHealthChecks(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddHealthChecks()
				.AddCheck<RemoteHealthCheck>("Remote endpoints health checks", failureStatus:Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy)
				.AddCheck<MemoryHealthCheck>("Service memory check", failureStatus:Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy)
				.AddUrlGroup(new Uri(configuration.GetValue<string>("HealthChecks:UrlGroup")??string.Empty));

			//Health checks UI
			services.AddHealthChecksUI(opt =>
			{
				opt.SetEvaluationTimeInSeconds(10);//time interval between checks
				opt.MaximumHistoryEntriesPerEndpoint(60); //max check history
				opt.SetApiMaxActiveRequests(1);// max api parallel request
				opt.AddHealthCheckEndpoint("", "api/health");
			})
				.AddInMemoryStorage();
			return services;
		}

	}
}
