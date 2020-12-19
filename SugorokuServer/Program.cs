using System;
using SugorokuLibrary;

namespace SugorokuServer
{
	class Program
	{
		static void Main(string[] args)
		{
			// Write Server Port
			var serverSocket = CreateTcpServerSocket.CreateServerSocket(9500);

			var clientSocket = AcceptTcpConnection.CreateClientSocket(serverSocket);
			var msg = HandleClient.ReceiveMessage(clientSocket);
		}
	}
}