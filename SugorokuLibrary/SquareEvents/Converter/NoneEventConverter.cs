using System;
using Newtonsoft.Json;

namespace SugorokuLibrary.SquareEvents.Converter
{
    public class NoneEventConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("eventType");
            writer.WriteValue("none");
            writer.WriteEndObject();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            return new NoneEvent();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(NoneEvent);
        }
    }
}