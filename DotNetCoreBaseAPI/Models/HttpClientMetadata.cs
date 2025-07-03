using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCoreBaseAPI.Models
{
	public class HttpClientMetadata
	{
		public List<HttpClientData> httpClients { get; set; }
	}

	public class HttpClientData
	{
		public string ClientName { get; set; }
		public string BaseUri { get; set; }
		public Dictionary<string,string> RequestHeaders { get; set; }
	}
}
