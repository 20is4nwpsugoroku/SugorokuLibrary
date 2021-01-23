using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using SugorokuLibrary.SquareEvents;

namespace SugorokuLibrary
{
    public class Field
    {
        public SquareEvent[] Squares { get; }

        public Field()
        {
            Squares = ParseSquares();
        }

        private static SquareEvent[] ParseSquares()
        {
            var asm = Assembly.GetExecutingAssembly();
            using var stream = asm.GetManifestResourceStream("SugorokuLibrary.squareData.json");
            using var reader = new StreamReader(stream!);

            var jsonString = reader.ReadToEnd();

            return JsonConvert.DeserializeObject<SquareEvent[]>(jsonString);
        }
    }
}