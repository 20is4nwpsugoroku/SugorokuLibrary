using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SugorokuLibrary.SquareEvents.Converter
{
    public class NextEventConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var nextEvent = (NextEvent) value!;
            writer.WriteStartObject();
            writer.WritePropertyName("eventType");
            writer.WriteValue("next");
            writer.WritePropertyName("count");
            writer.WriteValue(nextEvent.NextCount);
            writer.WritePropertyName("message");
            writer.WriteValue(nextEvent.Message);
            writer.WriteEndObject();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            return new NextEvent((int) jObject["count"]!, (string) jObject["message"]!);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(NextEvent);
        }
    }
}