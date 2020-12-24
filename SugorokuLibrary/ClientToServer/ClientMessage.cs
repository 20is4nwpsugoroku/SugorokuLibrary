using Newtonsoft.Json;
using SugorokuLibrary.ClientToServer.Converters;

namespace SugorokuLibrary.ClientToServer
{
	[JsonConverter(typeof(ClientMessageConverter))]
	public abstract class ClientMessage
	{
		public string MethodType { get; }
	}
}