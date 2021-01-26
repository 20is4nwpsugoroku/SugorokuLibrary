using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using DxLibDLL;
using SugorokuLibrary.ClientToServer;
using SugorokuLibrary.Protocol;

namespace SugorokuClient.Util
{
	public static class SocketManager
	{
		private static Socket socket { get; set; }
		public static string Address { get; private set; }
		public static int Port { get; private set; }
		public static bool IsConnected { get; private set; } = false;


		public static bool Connect(string address, int port)
		{
			Address = address;
			Port = port;
			try
			{
				IsConnected = false;
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.Connect(Address, Port);
				IsConnected = socket.Connected;
				return IsConnected;
			}
			catch(Exception)
			{
				return false;
			}	
		}


		public static bool Reconnect()
		{
			Close();
			return Connect(Address, Port);
		}


		public static void Close()
		{
			if(IsConnected)
			{
				socket.Shutdown(SocketShutdown.Both);
			}
			socket.Close();
			IsConnected = false;
		}


		public static (bool, string) SendRecv(string body)
		{
			if (!IsConnected)
			{
				if (Reconnect()) return (false, string.Empty);
			}
			try
			{
				var withHeader = HeaderProtocol.MakeHeader(body, true);
				var (s, r, recvMsg) = Connection.SendAndRecvMessage(withHeader, socket);
				Close();
				return (r, recvMsg);
			}
			catch (Exception)
			{
				return (false, string.Empty);
			}
		}

	}
}
