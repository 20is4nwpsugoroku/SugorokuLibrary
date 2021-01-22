using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using SugorokuLibrary.ClientToServer;
using SugorokuLibrary.Protocol;

namespace SugorokuClient.Util
{
	public static class SocketManager
	{
		private static Socket socket { get; set; }
		public static string Address { get; private set; }
		public static int Port { get; private set; }
		public static bool isReceived { get; private set; }
		public static bool isConnected { get; private set; }


		public static bool Connect(string address, int port)
		{
			Address = address;
			Port = port;
			try
			{
				isConnected = false;
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.ReceiveTimeout = 1000;
				socket.SendTimeout = 1000;
				socket.Connect(Address, Port);
				isConnected = true;
				return true;
			}
			catch(Exception e)
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
			try 
			{
				socket.Shutdown(SocketShutdown.Both);
			}
			catch(Exception)
			{
				DxLibDLL.DX.putsDx("Error");
			}
			socket.Close();
			isConnected = false;
		}


		public static (bool, string) SendRecv(string body)
		{
			if (isConnected)
			{
				if (!Reconnect()) return (false, string.Empty);
			}
			isReceived = false;
			var withHeader = HeaderProtocol.MakeHeader(body, true);
			DxLibDLL.DX.putsDx("Send" + withHeader);
			var (s, r, recvMsg) = Connection.SendAndRecvMessage(withHeader, socket);
			DxLibDLL.DX.putsDx("Send" + recvMsg);
			isReceived = r;
			return (r, recvMsg);
		}

	}
}
