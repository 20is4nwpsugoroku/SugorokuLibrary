using Newtonsoft.Json;

namespace SugorokuLibrary.ClientToServer
{
	[JsonConverter(typeof(GetAllMatchesConverter))]
	public class GetAllMatchesMessage : ClientMessage
	{
		[JsonProperty("methodType")] public string MethodType => "getAllMatches";
	}
}