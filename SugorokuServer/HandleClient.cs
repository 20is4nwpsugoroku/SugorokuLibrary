using System.Net.Sockets;
using System.Text;

namespace SugorokuServer
{
	public static class HandleClient
	{
		private const int RecvBufSize = 1024;

		public static string ReceiveMessage(Socket clientSocket)
		{
			var buf = new byte[RecvBufSize];
			var recvSize = clientSocket.Receive(buf);
			var msg = Encoding.UTF8.GetString(buf);
			var msgSize = GetBufSize(msg);

			while (recvSize >= msgSize)
			{
				buf = new byte[RecvBufSize];
				recvSize += clientSocket.Receive(buf);
				msg += Encoding.UTF8.GetString(buf);
			}

			return msg.TrimEnd('\0');
		}

		/// <summary>
		/// bodyのサイズを送信バッファの先頭に付与する
		/// </summary>
		/// <param name="bodyMessage"></param>
		/// <returns></returns>
		private static string MakeHeader(string bodyMessage)
		{
			return $"{bodyMessage.Length}\n{bodyMessage}";
		}

		/// <summary>
		/// 受信するバッファのサイズをヘッダから取得する
		/// サイズ = ヘッダに付与されているbodyのサイズ + ヘッダのサイズ + 1(改行コード)
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		private static int GetBufSize(string msg)
		{
			var sizeStr = msg.Split("\n")[0];
			return int.Parse(sizeStr) + sizeStr.Length + 1;
		}
	}
}