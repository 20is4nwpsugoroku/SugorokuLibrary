using Newtonsoft.Json;
using SugorokuLibrary.ServerToClient.Converters;

namespace SugorokuLibrary.ServerToClient
{
    [JsonConverter(typeof(ServerMessageConverter))]
    public class ServerMessage
    {
    }
}