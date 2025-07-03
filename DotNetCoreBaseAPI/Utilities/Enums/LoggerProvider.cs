using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCoreBaseAPI.Utilities.Enums
{
	public enum LoggerProvider
	{
		NONE=0,
		SERILOG,
		MICROSOFT_LOGGER,
		LOG4N
	}
}
