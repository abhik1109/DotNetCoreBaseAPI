using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DotNetCoreBaseAPI.Handlers
{
	public abstract class BasicAuthenticationHandlerBase : AuthenticationHandler<AuthenticationSchemeOptions>
	{
		public BasicAuthenticationHandlerBase(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
		{
		}
		
	}
}
