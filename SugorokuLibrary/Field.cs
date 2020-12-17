using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace SugorokuLibrary
{
	public class Field
	{
		public Square[] Squares { get; set; }
		public Player[] Players { get; set; }

		private Square[] ParseSquares()
		{
			using var reader = new StreamReader("squareData.json", Encoding.UTF8);
			var jsonString = reader.ReadToEnd();
			
			var options = new JsonSerializerOptions
			{
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
				PropertyNameCaseInsensitive = true
			};

			var squares = JsonSerializer.Deserialize<Dictionary<string, object>[]>(jsonString, options);

			return squares.Select(s =>
			{
				var square = new Square();
				square.Index = (int) s["index"];

				SquareEvent e = (string) s["eventType"] switch
				{
					"none" => new NoneEvent(),
					"prev" => new PrevEvent {BackCount = (int) s["count"]},
					"next" => new NextEvent {NextCount = (int) s["count"]},
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
