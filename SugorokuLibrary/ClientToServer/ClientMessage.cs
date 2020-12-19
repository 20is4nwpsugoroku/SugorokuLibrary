using Newtonsoft.Json;

namespace SugorokuLibrary.ClientToServer
{
	[JsonConverter(typeof(ClientMessageConverter))]
	public abstract class ClientMessage
	{
		public string MethodType { get; }
	}
}