using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SugorokuLibrary.SquareEvents;

namespace SugorokuLibrary
{
	public class Field
	{
		public static Square[] Squares => ParseSquares();

		private static Square[] ParseSquares()
		{
			var asm = Assembly.GetExecutingAssembly();
			using var stream = asm.GetManifestResourceStream("SugorokuLibrary.squareData.json");
			using var reader = new StreamReader(stream!);

			var jsonString = reader.ReadToEnd();

			var squares = JsonConvert.DeserializeObject<IEnumerable<Dictionary<string, JObject>>>(jsonString);

			return squares!.Select(s =>
			{
				var square = new Square {Index = (int) s["index"]};

				ISquareEvent e = (string) s["eventType"]! switch
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