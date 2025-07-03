using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCoreBaseAPI.Utilities.Policies
{
	public class ClientRetryPolicy
	{
		public AsyncRetryPolicy<HttpResponseMessage> ImmediateHttpRetry { get; }
		public AsyncRetryPolicy<HttpResponseMessage> LinearHttpRetry { get; }
		public AsyncRetryPolicy<HttpResponseMessage> ExponentialHttpRetry { get; }

		public ClientRetryPolicy()
		{
			ImmediateHttpRetry = Policy.HandleResult<HttpResponseMessage>
				(res=>!res.IsSuccessStatusCode).RetryAsync(5);//Retry count=5

			LinearHttpRetry = Policy.HandleResult<HttpResponseMessage>
				(res=>!res.IsSuccessStatusCode)
				.WaitAndRetryAsync(5, retryAttempt=> TimeSpan.FromSeconds(3));

			ExponentialHttpRetry = Policy.HandleResult<HttpResponseMessage>
				(res=>!res.IsSuccessStatusCode)
				.WaitAndRetryAsync(5, retryAttempt=>TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
		}
	}
}
