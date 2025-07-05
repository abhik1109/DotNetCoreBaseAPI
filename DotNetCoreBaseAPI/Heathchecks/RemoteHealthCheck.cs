using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCoreBaseAPI.Heathchecks
{
	public class RemoteHealthCheck : IHealthCheck
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration;

		public RemoteHealthCheck(IHttpClientFactory httpClientFactory, IConfiguration configuration)
		{
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
		}
		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
		{
			Dictionary<string, string> endpointHealth = new();
			using (var httpClient = _httpClientFactory.CreateClient())
			{
				var remoteEndPoints = _configuration.GetValue<List<string>>("HealthChceks:RemoteUris")??new();
				foreach(var uri in remoteEndPoints)
				{
					var response= await httpClient.GetAsync(uri);
					if (response.IsSuccessStatusCode) endpointHealth.Add(uri, "Healthy");
					else endpointHealth.Add(uri, "Unhealthy");
				}

				if (endpointHealth.ContainsValue("Unhealthy"))
				{
					return HealthCheckResult.Unhealthy($"Below endpoints are unhealthy\n{string.Join(",\n",endpointHealth.Where(x=>x.Value=="Unhealthy").Select(x=>x.Key))}");
				}
				else
				{
					return HealthCheckResult.Healthy("All remote endpoints are healthy");
				}
			}
		}
	}
}
