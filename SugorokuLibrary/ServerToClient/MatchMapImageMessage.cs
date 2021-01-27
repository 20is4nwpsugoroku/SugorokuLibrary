using Newtonsoft.Json;
using SugorokuLibrary.ServerToClient.Converters;

namespace SugorokuLibrary.ServerToClient
{
    [JsonConverter(typeof(MatchMapImageMessageConverter))]
    public class MatchMapImageMessage : ServerMessage
    {
        public override string MethodType => "matchMapImage";
        public string ImageBase64 { get; }

        public MatchMapImageMessage(string imageBase64)
        {
            ImageBase64 = imageBase64;
        }
    }
}