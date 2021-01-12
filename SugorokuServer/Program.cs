using System.Net.Sockets;
using System.Threading.Tasks;

namespace SugorokuServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			// Write Server Port
			var serverSocket = CreateTcpServerSocket.CreateServerSocket(9500);

			var client = new HandleClient();
			while (true)
			{
				var clientSocket = AcceptTcpConnection.CreateClientSocket(serverSocket);
				Task.Run(() => Connection(clientSocket, client));
			}
		}

		private static void Connection(Socket clientSocket, HandleClient handleClient)
		{
			var recvMsg = HandleClient.ReceiveMessage(clientSocket);
			var sendMsg = handleClient.MakeSendMessage(recvMsg);

			HandleClient.SendMessage(clientSocket, sendMsg);
		}
	}
}