using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SugorokuLibrary.ServerToClient.Converters
{
    public class FailedMessageConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var failed = (FailedMessage) value!;
            
            writer.WriteStartObject();
            writer.WritePropertyName("methodType");
            writer.WriteValue(failed.MethodType);
            writer.WritePropertyName("message");
            writer.WriteValue(failed.Message);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            return new FailedMessage((string) jObject["message"]!);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(FailedMessage);
        }
    }
}