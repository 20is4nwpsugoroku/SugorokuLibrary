using System;
using System.Linq;
using System.Text;

namespace SugorokuLibrary.Protocol
{
	public static class HeaderProtocol
	{
		/// <summary>
		/// bodyのサイズを送信バッファの先頭に付与する
		/// </summary>
		/// <param name="bodyMessage">送信内容</param>
		/// <param name="methodSuccess">了承か不正操作か</param>
		/// <returns></returns>
		public static string MakeHeader(string bodyMessage, bool methodSuccess)
		{
			var bodyLength = Encoding.UTF8.GetByteCount(bodyMessage);
			var headerLength = bodyLength.ToString().Length + (methodSuccess ? 4 : 6);
			return $"{headerLength + bodyLength},{(methodSuccess ? "OK" : "FAIL")}\n{bodyMessage}";
		}

		/// <summary>
		/// 受信するバッファのサイズをヘッダから取得する
		/// サイズ = ヘッダに付与されているbodyのサイズ + ヘッダのサイズ + 1(改行コード)
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		public static (int, bool, string) ParseHeader(string msg)
		{
			var lines = msg.Split("\n");
			var headerSplit = lines[0].Split(',');
			var (sizeStr, functionSuccess, bodyLines) = (headerSplit[0], headerSplit[1], string.Concat(lines.Skip(1)));
			return (int.Parse(sizeStr) + lines[0].Length + 1, functionSuccess == "OK", bodyLines);
		}

		public static (int, bool) AnalyzeHeader(string header)
		{
			var headerSplit = header.Split(',');

			Console.WriteLine($"Parsing Size: {headerSplit[0]}");
			return (int.Parse(headerSplit[0]), headerSplit[1] == "OK");
		}
	}
}