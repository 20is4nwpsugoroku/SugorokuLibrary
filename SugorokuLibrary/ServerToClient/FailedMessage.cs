using Newtonsoft.Json;
using SugorokuLibrary.ServerToClient.Converters;

namespace SugorokuLibrary.ServerToClient
{
    [JsonConverter(typeof(FailedMessageConverter))]
    public class FailedMessage
    {
        [JsonProperty("methodType")] public string MethodType => "failed";
        
        [JsonProperty("message")] public string Message { get; }

        public FailedMessage(string message)
        {
            Message = message;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is FailedMessage fa)) return false;
            return fa.Message == Message;
        }

        public override int GetHashCode()
        {
            return Message.GetHashCode();
        }
    }
}