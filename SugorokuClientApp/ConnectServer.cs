using System;
using System.Net;
using System.Net.Sockets;

namespace SugorokuClientApp
{
	public static class ConnectServer
	{
		public static Socket CreateSocket(IPAddress serverIpAddress, int serverPort)
		{
			var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				socket.Connect(serverIpAddress, serverPort);
				return socket;
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
				throw;
			}
		}
	}
}