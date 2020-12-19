namespace SugorokuLibrary.ClientToServer
{
	public class CloseCreateMessage : IClientMessage
	{
		public string MethodType => "closeCreate";
		
		public string FieldKey { get; }

		public CloseCreateMessage(string fieldKey)
		{
			FieldKey = fieldKey;
		}
	}
}