using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace SugorokuLibrary.Protocol
{
	public static class Connection
	{
		private const int MaxSize = 1024;

		private static (int, bool, string) Receive(Socket partnerSocket)
		{
			var buf = new byte[MaxSize];
			var receivedMessages = new List<byte>();
			var receivedSize = partnerSocket.Receive(buf);
			if (receivedSize == 0)
			{
				Console.WriteLine("ZERO!!!!!!!!");
			}
			receivedMessages.AddRange(buf);
			var headerBytes = buf.TakeWhile(b => b != '\n').ToList();
			var (bodySize, result) = HeaderProtocol.AnalyzeHeader(Encoding.UTF8.GetString(headerBytes.ToArray()));

			while (receivedSize < bodySize)
			{
				buf = new byte[MaxSize];
				receivedSize += partnerSocket.Receive(buf, receivedSize, MaxSize - receivedSize, SocketFlags.None);
				receivedMessages.AddRange(buf);
			}

			var msg = Encoding.UTF8.GetString(receivedMessages.TakeWhile(b => b != 0).ToArray());
			return (bodySize, result, msg);
		}

		private static void Send(string message, Socket partnerSocket)
		{
			var sendSize = 0;
			while (sendSize < message.Length)
			{
				var startSelected = message[sendSize..];
				Console.WriteLine(startSelected);
				sendSize += partnerSocket.Send(Encoding.UTF8.GetBytes(startSelected));
			}
		}

		public static (int, bool, string) SendAndRecvMessage(string message, Socket partnerSocket)
		{
			Send(message, partnerSocket);
			return Receive(partnerSocket);
		}

		public static void RecvAndSendMessage(string message, Socket partnerSocket)
		{
			Send(message, partnerSocket);
		}
	}
}