using System.Collections.Generic;
using Newtonsoft.Json;
using SugorokuLibrary.ServerToClient.Converters;

namespace SugorokuLibrary.ServerToClient
{
    [JsonConverter(typeof(DiceResultMessageConverter))]
    public class DiceResultMessage : ServerMessage
    {
        public override string MethodType => "diceResult";
        public int Dice { get; }
        public string Message { get; }
        public int FirstPosition { get; }
        public int FinalPosition { get; }
        
        public IEnumerable<int>? Ranking { get; set; }

        public DiceResultMessage(int dice, string message, int firstPosition, int finalPosition)
        {
            Dice = dice;
            Message = message;
            FirstPosition = firstPosition;
            FinalPosition = finalPosition;
        }
    }
}