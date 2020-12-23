using System;
using Newtonsoft.Json;

namespace SugorokuLibrary.ClientToServer
{
	public class GetAllMatchesConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
		{
			var getAllMatch = (GetAllMatchesMessage) value!;

			writer.WriteStartObject();
			writer.WritePropertyName("methodType");
			writer.WriteValue(getAllMatch.MethodType);
			writer.WriteEndObject();
		}

		public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
			JsonSerializer serializer)
		{
			return new GetAllMatchesMessage();
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(GetAllMatchesMessage);
		}
	}
}