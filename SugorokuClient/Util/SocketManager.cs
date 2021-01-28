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
	/// <summary>
	/// ソケット通信の管理を行うクラス
	/// </summary>
	public static class SocketManager
	{
		public static string Address { get; private set; }
		public static int Port { get; private set; }

		private static bool BeforeSetAddress { get; set; } = true;

		public static void SetAddress(string address, int port)
		{
			Address = address;
			Port = port;
			BeforeSetAddress = false; ;
		}


		private static Socket Reconnect(Socket socket)
		{
			socket.Close();
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Connect(Address, Port);
			return socket;
		}


		public static (bool, string) SendRecv(string body)
		{
			if (BeforeSetAddress) return (false, string.Empty);
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Connect(Address, Port);
			if (!socket.Connected)
			{
				while(!socket.Connected)
				{
					socket = Reconnect(socket);
				}
			}
			try
			{
				var withHeader = HeaderProtocol.MakeHeader(body, true);
				var (s, r, recvMsg) = Connection.SendAndRecvMessage(withHeader, socket);
				socket.Close();
				return (r, recvMsg);
			}
			catch (Exception)
			{
				return (false, string.Empty);
			}
		}
	}
}
