using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
