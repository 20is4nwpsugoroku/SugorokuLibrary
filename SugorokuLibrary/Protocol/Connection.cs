using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace SugorokuLibrary.Protocol
{
	public static class Connection
	{
		private const int MaxSize = 1024;

		public static (int, bool, string) Receive(Socket partnerSocket)
		{
			var buf = new byte[16_777_216];
			var receivedSize = partnerSocket.Receive(buf, 0, MaxSize, SocketFlags.None);
			var headerBytes = buf.TakeWhile(b => b != '\n').ToList();
			var (bodySize, _) = HeaderProtocol.AnalyzeHeader(Encoding.UTF8.GetString(headerBytes.ToArray()));

			while (receivedSize < bodySize)
			{
				var maxSize = receivedSize + MaxSize > bodySize
					? bodySize - receivedSize
					: MaxSize;
				receivedSize += partnerSocket.Receive(buf, receivedSize, maxSize, SocketFlags.None);
			}

			var msg = Encoding.UTF8.GetString(buf.TakeWhile(b => b != 0).ToArray());
			return HeaderProtocol.ParseHeader(msg);
		}

		public static void Send(string message, Socket partnerSocket, bool makeHeader = false)
		{
			var sendSize = 0;
			if (makeHeader)
			{
				message = HeaderProtocol.MakeHeader(message, true);
			}

			var sendMsgBytes = Encoding.UTF8.GetBytes(message);
			while (sendSize < sendMsgBytes.Length)
			{
				var maxSize = sendSize + MaxSize > sendMsgBytes.Length
					? sendMsgBytes.Length - sendSize
					: MaxSize;
				sendSize += partnerSocket.Send(sendMsgBytes, sendSize, maxSize, SocketFlags.None);
			}
		}

		public static (int, bool, string) SendAndRecvMessage(string message, Socket partnerSocket,
			bool makeHeader = false)
		{
			Send(message, partnerSocket, makeHeader);
			return Receive(partnerSocket);
		}
	}
}