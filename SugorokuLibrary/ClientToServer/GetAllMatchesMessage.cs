using Newtonsoft.Json;
using SugorokuLibrary.ClientToServer.Converters;

namespace SugorokuLibrary.ClientToServer
{
	[JsonConverter(typeof(GetAllMatchesConverter))]
	public class GetAllMatchesMessage : ClientMessage
	{
		[JsonProperty("methodType")] public string MethodType => "getAllMatches";
	}
}