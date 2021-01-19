using Newtonsoft.Json;
using SugorokuLibrary.ServerToClient.Converters;

namespace SugorokuLibrary.ServerToClient
{
	[JsonConverter(typeof(DiceResultMessageConverter))]
	public class DiceResultMessage : ServerMessage
	{
		[JsonProperty("methodType")] public string MethodType => "diceResult";
		[JsonProperty("dice")] public int Dice { get; }
		[JsonProperty("squareEvent")] public string SquareEvent { get; }
		[JsonProperty("finalPosition")] public int FinalPosition { get; }

		public DiceResultMessage(int dice, string squareEvent, int finalPosition)
		{
			Dice = dice;
			SquareEvent = squareEvent;
			FinalPosition = finalPosition;
		}
	}
}