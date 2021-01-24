using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SugorokuLibrary.ClientToServer.Converters
{
    public class GetRankingConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var getRanking = (GetRankingMessage) value!;
            
            writer.WriteStartObject();
            writer.WritePropertyName("methodType");
            writer.WriteValue(getRanking.MethodType);
            writer.WritePropertyName("matchKey");
            writer.WriteValue(getRanking.MatchKey);
            writer.WriteEndObject();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            return new GetRankingMessage((string) jObject["matchKey"]!);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GetRankingMessage);
        }
    }
}