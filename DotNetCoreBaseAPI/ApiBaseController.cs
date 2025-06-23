using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotNetCoreBaseAPI
{
	public abstract class ApiBaseController<T> : ControllerBase
	{
		private readonly ILogger<T> _logger;

		public ApiBaseController(ILogger<T> logger)
        {
            _logger = logger;
        }
    }
}
