using System;
using System.Net;
using System.Net.Sockets;

namespace SugorokuServer
{
	public static class AcceptTcpConnection
	{
		public static Socket CreateClientSocket(Socket serverSocket)
		{
			try
			{
				var clientSocket = serverSocket.Accept();
				var remoteEndPoint = (IPEndPoint) clientSocket.RemoteEndPoint;
				Console.WriteLine(
					$"Handling client: {IPAddress.Parse(remoteEndPoint.Address.ToString())}:{remoteEndPoint.Port}");

				return clientSocket;
			}
			catch (Exception e)
			{
				Console.WriteLine("Accept() failed: " + e);
				throw;
			}
		}
	}
}