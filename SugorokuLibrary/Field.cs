using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace SugorokuLibrary
{
	public class Field
	{
		public Field()
		{
			Squares = ParseSquares();
		}

		public Square[] Squares { get; }
		public Player[]? Players { get; set; }

		private static Square[] ParseSquares()
		{
			var asm = Assembly.GetExecutingAssembly();
			using var stream = asm.GetManifestResourceStream("SugorokuLibrary.squareData.json");
			using var reader = new StreamReader(stream!);

			var jsonString = reader.ReadToEnd();

			var options = new JsonSerializerOptions
			{
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
				PropertyNameCaseInsensitive = true
			};

			var squares = JsonSerializer.Deserialize<Dictionary<string, JsonElement>[]>(jsonString, options);

			return squares!.Select(s =>
			{
				var square = new Square {Index = s["index"].GetInt32()};

				SquareEvent e = s["eventType"].GetString() switch
				{
					"none" => new NoneEvent(),
					"prev" => new PrevEvent {BackCount = s["count"].GetInt32()},
					"next" => new NextEvent {NextCount = s["count"].GetInt32()},
					"prevDice" => new PrevDiceEvent(),
					"wait" => new WaitEvent(),
					"goStart" => new GoStartEvent(),
					"diceAgain" => new DiceAgainEvent(),
					_ => throw new ArgumentOutOfRangeException()
				};

				square.Event = e;
				return square;
			}).ToArray();
		}
	}
}