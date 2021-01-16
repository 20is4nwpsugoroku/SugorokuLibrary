using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SugorokuLibrary.ServerToClient.Converters
{
    public class ServerMessageConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            JsonConverter converter = (string) jObject["methodType"]! switch
            {
                "failed" => new FailedMessageConverter(),
                "diceResult" => new DiceResultMessageConverter(),
                _ => throw new ArgumentException()
            };
            var newReader = jObject.CreateReader();
            return converter.ReadJson(newReader, objectType, existingValue, serializer);
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(ServerMessage);
        }

        public override bool CanWrite => false;
    }
}