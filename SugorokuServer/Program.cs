using System.Net.Sockets;
using System.Threading.Tasks;
using SugorokuLibrary.Protocol;

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
				Task.Run(() => Communication(clientSocket, client));
			}
		}

		private static void Communication(Socket clientSocket, HandleClient handleClient)
		{
			var (_, _, recvMsg) = Connection.Receive(clientSocket);
			var sendMsg = handleClient.MakeSendMessage(recvMsg);

			Connection.Send(sendMsg, clientSocket);
			clientSocket.Close();
		}
	}
}