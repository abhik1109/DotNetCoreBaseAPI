using DotNetCoreBaseAPI.Utilities.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCoreBaseAPI.Extensions
{
	public static class WebApplicationBuilderExtensions
	{
		public static WebApplicationBuilder RegisterLogging(this WebApplicationBuilder builder, LoggerProvider loggerProvider)
		{
			switch (loggerProvider)
			{
				case LoggerProvider.SERILOG:
					//Configure Serilog logger
					var logger = new LoggerConfiguration()
						.ReadFrom.Configuration(builder.Configuration)
						.Enrich.FromLogContext()
						.CreateLogger();

					builder.Logging.ClearProviders();
					builder.Logging.AddSerilog(logger);
					break;

				case LoggerProvider.MICROSOFT_LOGGER:
					builder.Logging.ClearProviders();
					builder.Logging.AddConsole();
					builder.Logging.AddDebug();
					break;

				default:

					break;
			}
			return builder;
		}
	}
}
