using Newtonsoft.Json;
using SugorokuLibrary.ServerToClient.Converters;

namespace SugorokuLibrary.ServerToClient
{
    [JsonConverter(typeof(ServerMessageConverter))]
    public abstract class ServerMessage
    {
        public abstract string MethodType { get; }
    }
}