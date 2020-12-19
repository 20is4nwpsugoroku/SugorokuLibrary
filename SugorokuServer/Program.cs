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
				Task.Run(() => Connection(serverSocket, client));
			}
		}

		private static void Connection(Socket serverSocket, HandleClient handleClient)
		{
			var clientSocket = AcceptTcpConnection.CreateClientSocket(serverSocket);
			var recvMsg = HandleClient.ReceiveMessage(clientSocket);
			var sendMsg = handleClient.MakeSendMessage(recvMsg);

			HandleClient.SendMessage(clientSocket, sendMsg);
		}
	}
}