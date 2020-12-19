using System;
using System.Net;
using System.Net.Sockets;

namespace SugorokuServer
{
	public static class CreateTcpServerSocket
	{
		public static Socket CreateServerSocket(int port)
		{
			var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			var echoServerAddr = new IPEndPoint(IPAddress.Any, port);

			try
			{
				socket.Bind(echoServerAddr);
			}
			catch (Exception e)
			{
				Console.WriteLine("Bind() failed: " + e);
				throw;
			}

			try
			{
				socket.Listen(5);
			}
			catch (Exception e)
			{
				Console.WriteLine("Listen() failed: " + e);
				throw;
			}

			return socket;
		}
	}
}